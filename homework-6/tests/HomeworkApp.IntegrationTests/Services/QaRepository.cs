using System.Threading;
using System.Threading.Tasks;
using Dapper;
using HomeworkApp.Dal.Repositories;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.IntegrationTests.Services;

public class QaRepository : PgRepository
{
    public QaRepository(IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }

    public async Task SetParentTask(long id, long parentTaskId, CancellationToken token)
    {
        const string sqlQuery = @"
update tasks
set parent_task_id = @ParentTaskId
where id = @Id
";

        var command = new CommandDefinition(
            sqlQuery,
            new
            {
                Id = id,
                ParentTaskId = parentTaskId
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);

        await using var connection = await GetConnection();
        await connection.QueryAsync(command);
    }
}