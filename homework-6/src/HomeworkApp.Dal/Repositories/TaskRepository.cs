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

public class TaskRepository : PgRepository, ITaskRepository
{
    public TaskRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task<long[]> Add(TaskEntityV1[] tasks, CancellationToken token)
    {
        const string sqlQuery = @"
insert into tasks (parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at)
select parent_task_id, number, title, description, status, created_at, created_by_user_id, assigned_to_user_id, completed_at
  from UNNEST(@Tasks)
returning id;
";

        await using var connection = await GetConnection();
        var ids = await connection.QueryAsync<long>(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Tasks = tasks
                },
                cancellationToken: token));

        return ids
            .ToArray();
    }

    public async Task<TaskEntityV1[]> Get(TaskGetModel query, CancellationToken token)
    {
        var baseSql = @"
select id
     , parent_task_id
     , number
     , title
     , description
     , status
     , created_at
     , created_by_user_id
     , assigned_to_user_id
     , completed_at
  from tasks
";

        var conditions = new List<string>();
        var @params = new DynamicParameters();

        if (query.TaskIds.Any())
        {
            conditions.Add($"id = ANY(@TaskIds)");
            @params.Add($"TaskIds", query.TaskIds);
        }

        var cmd = new CommandDefinition(
            baseSql + $" WHERE {string.Join(" AND ", conditions)} ",
            @params,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();
        return (await connection.QueryAsync<TaskEntityV1>(cmd))
            .ToArray();
    }

    public async Task Assign(AssignTaskModel model, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
   set assigned_to_user_id = @AssignToUserId
     , status = @Status
 where id = @TaskId
";

        await using var connection = await GetConnection();
        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    TaskId = model.TaskId,
                    AssignToUserId = model.AssignToUserId,
                    Status = model.Status
                },
                cancellationToken: token));
    }

    public async Task<SubTasksGetModel[]> GetSubTasksInStatus(long parentTaskId, int[] statuses, CancellationToken token)
    {
        const string sqlQuery = @"
with recursive t(task_id, title, status, parent_task_ids) as (
    select
        id as task_id,
        title,
        status,
        array[parent_task_id] as parent_task_ids
    from
        tasks
    where
        parent_task_id = @ParentTaskId
    
    union all
    
    select
        tasks.id as task_id,
        tasks.title,
        tasks.status,
        parent_task_ids || tasks.parent_task_id
    from
        tasks join t ON tasks.parent_task_id = t.task_id
)
select * from t
where t.status = any(@Statuses)
";
        var command = new CommandDefinition(
            sqlQuery,
            new
            {
                ParentTaskId = parentTaskId,
                Statuses = statuses
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();
        var subTasks = await connection.QueryAsync<SubTasksGetModel>(command);

        return subTasks.ToArray();
    }
}
