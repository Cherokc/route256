using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    public TaskCommentRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var baseSql = @"
select id
     , task_id
     , author_user_id
     , message
     , at
     , modified_at
     , deleted_at
  from task_comments
";
        var conditions = new List<string>();
        var @params = new DynamicParameters();
        
        conditions.Add("task_id = @TaskId");
        @params.Add($"TaskId", model.TaskId);
        
        if (!model.IncludeDeleted)
        {
            conditions.Add($"deleted_at is null");
        }

        var cmd = new CommandDefinition(
            $"{baseSql} WHERE {string.Join(" AND ", conditions)} order by at desc",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskCommentEntityV1>(cmd))
            .ToArray();
    }
    
    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
insert into task_comments (task_id, author_user_id, message, at, deleted_at) 
values (@TaskId, @AuthorUserId, @Message, @At, @DeletedAt)
returning id;
";
        
        await using var connection = await GetConnection();
        var id = await connection.QuerySingleAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AuthorUserID = model.AuthorUserId,
                    Message = model.Message,
                    At = model.At,
                    DeletedAt = model.DeletedAt
                },
                cancellationToken: token));
        
        return id;
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
   set message = @Message
     , modified_at = @ModifiedAt
 where id = @Id
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Message = model.Message,
                    ModifiedAt = model.ModifiedAt,
                    Id = model.Id
                },
                cancellationToken: token));
    }

    public async Task SetDeleted(TaskCommentDeleteModel model, CancellationToken token)
    {
        const string sqlQuery = @"
update task_comments
set deleted_at = @DeletedAt
where id = @Id
";
        
        await using var connection = await GetConnection();
        await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    DeletedAt = model.DeletedAt,
                    Id = model.CommentId
                },
                cancellationToken: token));
    }
}