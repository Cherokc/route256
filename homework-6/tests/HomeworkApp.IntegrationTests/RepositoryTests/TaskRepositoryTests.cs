using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using HomeworkApp.IntegrationTests.Services;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private const int ROOTS_COUNT = 2;
    private const int CHILDREN_COUNT = 5;

    private readonly ITaskRepository _repository;
    private readonly QaRepository _qaRepository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
        _qaRepository = fixture.QaRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);

        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }

    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);

        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();

        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);

        // Act
        await _repository.Assign(assign, default);

        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        results.Should().HaveCount(1);
        var task = results.Single();

        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }
    
    
    [Fact]
    public async Task GetSubTasksInAnyStatus_Success()
    {
        var (tasks, parentTaskId1) = await FillTasks();

        // Act
        var statuses = 
            Enum
                .GetValues<HomeworkApp.Bll.Enums.TaskStatus>()
                .Select(s => (int)s)
                .ToArray();
        var actualSubTasks = await _repository.GetSubTasksInStatus(
            parentTaskId1,
            statuses,
            default);

        // Asserts
        var expectedSubTasksPath = tasks
            .Where(t => t.Id != parentTaskId1)
            .Take(CHILDREN_COUNT - 1)
            .Aggregate(new List<long>(), (acc, t) =>
            {
                acc.Add(t.ParentTaskId!.Value);
                return acc;
            })
            .ToArray();

        var expectedSubTasks = tasks
            .Where(t => t.Id != parentTaskId1)
            .Take(CHILDREN_COUNT - 1)
            .Select((t, i) => new SubTasksGetModel
            {
                TaskId = t.Id,
                Title = t.Title,
                Status = t.Status,
                ParentTaskIds = expectedSubTasksPath[..(i + 1)]
            })
            .ToArray();

        actualSubTasks.Should().BeEquivalentTo(expectedSubTasks);
        actualSubTasks.Should().NotContain(t => t.TaskId == parentTaskId1);
        actualSubTasks.Should().Contain(t => t.ParentTaskIds.Any());
        foreach (var actualSubTask in actualSubTasks)
        {
            var expectedSubTask = expectedSubTasks.Single(t => t.TaskId == actualSubTask.TaskId);
            actualSubTask.ParentTaskIds.Should().BeEquivalentTo(expectedSubTask.ParentTaskIds);
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task GetSubTasksInTheStatus_Success(int status)
    {
        // Arrange
        var filledTasks = await FillTasks(status);

        // Act
        var actualSubTasks = await _repository.GetSubTasksInStatus(
            filledTasks.FirstParentTaskId,
            new[] { status },
            default);

        // Asserts

        actualSubTasks.Should().OnlyContain(t => t.Status == status);
    }

    private async Task<(TaskEntityV1[] Tasks, long FirstParentTaskId)> FillTasks(int? status = null)
    {
        var tasks = TaskEntityV1Faker.Generate(ROOTS_COUNT * CHILDREN_COUNT, status);
        var taskIds = await _repository.Add(tasks, default);

        for (var i = 0; i < ROOTS_COUNT; i++)
        {
            for (var j = 0; j < CHILDREN_COUNT; j++)
            {
                var index = i * CHILDREN_COUNT + j;

                var id = taskIds[index];
                tasks[index] = tasks[index].WithId(id);

                if (j is 0) continue;

                var parentTaskId = taskIds[index - 1];
                await _qaRepository.SetParentTask(id, parentTaskId, default);

                tasks[index] = tasks[index].WithParentTaskId(parentTaskId);
            }
        }

        return (tasks, taskIds[0]);
    }
}
