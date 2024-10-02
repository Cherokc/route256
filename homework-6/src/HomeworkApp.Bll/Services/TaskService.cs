using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using AssignTaskModel = HomeworkApp.Dal.Models.AssignTaskModel;
using TaskStatus = HomeworkApp.Bll.Enums.TaskStatus;

namespace HomeworkApp.Bll.Services;

public class TaskService : ITaskService
{
    const int CacheLifeTimeSeconds = 5;
    const int TaskCommentsToShowNumber = 5;

    private readonly ILogger<TaskService> _logger;
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskLogRepository _taskLogRepository;
    private readonly ITakenTaskRepository _takenTaskRepository;
    private readonly ITaskCommentRepository _taskCommentRepository;
    private readonly IDistributedCache _distributedCache;

    public TaskService(
        ILogger<TaskService> logger,
        ITaskRepository taskRepository,
        ITaskLogRepository taskLogRepository,
        ITakenTaskRepository takenTaskRepository,
        ITaskCommentRepository taskCommentRepository,
        IDistributedCache distributedCache)
    {
        _logger = logger;
        _taskRepository = taskRepository;
        _taskLogRepository = taskLogRepository;
        _takenTaskRepository = takenTaskRepository;
        _taskCommentRepository = taskCommentRepository;
        _distributedCache = distributedCache;
    }

    public async Task<long> CreateTask(
        CreateTaskModel model,
        CancellationToken token)
    {
        using var transaction = CreateTransactionScope();

        var task =  new TaskEntityV1
        {
            Title = model.Title,
            Description = model.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = model.UserId,
            Status = (int) Enums.TaskStatus.Draft,
            Number = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
        };
        var taskId = (await _taskRepository.Add(
                new[] { task },
                token))
            .Single();

        var taskLog = new TaskLogEntityV1
        {
            TaskId = taskId,
            Number = task.Number,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            At = task.CreatedAt,
            UserId = model.UserId
        };;
        await _taskLogRepository.Add(new[] { taskLog }, token);

        transaction.Complete();

        return taskId;
    }

    public async Task<GetTaskModel?> GetTask(
        long taskId,
        CancellationToken token)
    {
        var cacheKey = $"cache_tasks:{taskId}";
        var cachedTask = await _distributedCache.GetStringAsync(cacheKey, token);
        if (!string.IsNullOrEmpty(cachedTask))
        {
            return JsonSerializer.Deserialize<GetTaskModel>(cachedTask);
        }

        var task = (await _taskRepository.Get(new TaskGetModel
            {
                TaskIds = new[] {taskId}
            }, token))
            .SingleOrDefault();

        if (task is null)
        {
            return null;
        }

        var result = new GetTaskModel
        {
            TaskId = task.Id,
            Number = task.Number,
            AssignedToUserId = task.AssignedToUserId,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            CreatedByUserId = task.CreatedByUserId,
            Description = task.Description,
            ParentTaskId = task.ParentTaskId,
            Status = (TaskStatus)task.Status,
            Title = task.Title
        };

        var taskJson = JsonSerializer.Serialize(result);
        await _distributedCache.SetStringAsync(
            cacheKey,
            taskJson,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            },
            token);

        return result;
    }

    public async Task AssignTask(
        Bll.Models.AssignTaskModel model,
        CancellationToken token)
    {
        var task = (await _taskRepository.Get(new TaskGetModel
            {
                TaskIds = new[] {model.TaskId}
            }, token))
            .SingleOrDefault();

        if (task is null)
        {
            return;
        }

        using var transaction = CreateTransactionScope();

        await _taskRepository.Assign(
            new AssignTaskModel()
            {
                TaskId = model.TaskId,
                AssignToUserId = model.AssignToUserId,
                Status = (int)Enums.TaskStatus.InProgress
            },
            token);

        task = task with
        {
            Status = (int)Enums.TaskStatus.InProgress,
            AssignedToUserId = model.AssignToUserId
        };

        var taskLog = new TaskLogEntityV1
        {
            TaskId = task.Id,
            Number = task.Number,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            AssignedToUserId = task.AssignedToUserId.Value,
            At = DateTimeOffset.UtcNow,
            UserId = model.UserId,
        };
        await _taskLogRepository.Add(new[] { taskLog }, token);

        await _takenTaskRepository.Add(new TakenTaskModel()
        {
            TaskId = task.Id,
            Title = task.Title,
            AssignedToUserId = task.AssignedToUserId.Value,
            AssignedAt = taskLog.At
        }, token);

        transaction.Complete();
    }

    public async Task<long> CreateComment(CreateTaskCommentModel model, CancellationToken token)
    {
        var taskComment =  new TaskCommentEntityV1()
        {
            TaskId = model.TaskId,
            AuthorUserId = model.AuthorUserId,
            Message = model.Message,
            At = model.At
        };

        var taskCommentId = await _taskCommentRepository.Add(taskComment, token);
        
        var taskMessage = new GetTaskCommentsModel
        {
            TaskId = model.TaskId,
            Message = model.Message,
            At = model.At,
            IsDeleted = false
        };

        var messages = new List<GetTaskCommentsModel>() { taskMessage };
        
        var cacheKey = $"cached_task_comments:{model.TaskId}";
        var cachedTaskComments = await _distributedCache.GetStringAsync(cacheKey, token);

 
        try
        {
            var cachedComments = _distributedCache.GetString(cacheKey);
            if (!string.IsNullOrEmpty(cachedComments))
            {
                messages.AddRange(JsonSerializer.Deserialize<GetTaskCommentsModel[]>(cachedComments));
            }
        }
        catch (JsonException e)
        {
            _logger.LogError(
                e,
                "Deserialization error, cacheKey:{CacheKey}, comments:{CachedTaskComments}", 
                cacheKey, 
                cachedTaskComments);
        }

        var taskCommentsJson = JsonSerializer.Serialize(messages.Take(TaskCommentsToShowNumber).ToArray());
        await _distributedCache.SetStringAsync(
            cacheKey,
            taskCommentsJson,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheLifeTimeSeconds)
            }, token);

        return taskCommentId;
    }

    public async Task<GetTaskCommentsModel[]> GetTaskComments(long taskId, CancellationToken token)
    {
        var cacheKey = $"cached_task_comments:{taskId}";
        var cachedTaskComments = await _distributedCache.GetStringAsync(cacheKey, token);
        if (!string.IsNullOrEmpty(cachedTaskComments))
        {
            try
            {
                var comments = JsonSerializer.Deserialize<GetTaskCommentsModel[]>(cachedTaskComments);
                if (comments != null)
                {
                    return comments;
                }
            }
            catch (JsonException e)
            {
                await _distributedCache.RemoveAsync(cacheKey, token);
                _logger.LogError(
                    e,
                    "Deserialization error, cacheKey:{CacheKey}, comments:{CachedTaskComments}", 
                    cacheKey, 
                    cachedTaskComments);
            }
        }
        
        var taskComments = await _taskCommentRepository.Get(new TaskCommentGetModel
        {
            TaskId = taskId,
            IncludeDeleted = false
        }, token);
        
        var result = taskComments
            .Take(TaskCommentsToShowNumber)
            .Select(tc => new GetTaskCommentsModel
            {
                TaskId = tc.TaskId,
                Message = tc.Message,
                At = tc.At,
                IsDeleted = tc.DeletedAt is not null
            })
            .ToArray();

        var taskCommentsJson = JsonSerializer.Serialize(result);
        await _distributedCache.SetStringAsync(
            cacheKey, 
            taskCommentsJson,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CacheLifeTimeSeconds)
            },
            token);
        
        return result;
    }


    private TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(5)
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}
