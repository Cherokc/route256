using System.IO;
using FluentMigrator.Runner;
using HomeworkApp.Bll.Extensions;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Extensions;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace HomeworkApp.IntegrationTests.Fixtures
{
    public class TestFixture
    {
        public IUserRepository UserRepository { get; }

        public ITaskRepository TaskRepository { get; }

        public ITaskLogRepository TaskLogRepository { get; }

        public ITakenTaskRepository TakenTaskRepository { get; }
        
        public ITaskCommentRepository TaskCommentRepository { get; }

        public IUserScheduleRepository UserScheduleRepository { get; }
        
        public IRateLimiterService RateLimiterService { get; }
        
        public QaRepository QaRepository { get; }

        public TestFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(
                    services =>
                    {
                        services
                            .AddBllServices()
                            .AddDalInfrastructure(config)
                            .AddDalRepositories();

                        services.AddScoped<QaRepository>();
                        
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = config["DalOptions:RedisConnectionString"];
                        });
                    })
                .Build();

            ClearDatabase(host);
            host.MigrateUp();

            var scope = host.Services.CreateScope();
            UserRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            TaskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
            TaskLogRepository = scope.ServiceProvider.GetRequiredService<ITaskLogRepository>();
            TakenTaskRepository = scope.ServiceProvider.GetRequiredService<ITakenTaskRepository>();
            UserScheduleRepository = scope.ServiceProvider.GetRequiredService<IUserScheduleRepository>();
            TaskCommentRepository = scope.ServiceProvider.GetRequiredService<ITaskCommentRepository>();
            RateLimiterService = scope.ServiceProvider.GetRequiredService<IRateLimiterService>();
            QaRepository = scope.ServiceProvider.GetRequiredService<QaRepository>();
            
            FluentAssertionOptions.UseDefaultPrecision();
        }

        private static void ClearDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(0);
        }
    }
}
