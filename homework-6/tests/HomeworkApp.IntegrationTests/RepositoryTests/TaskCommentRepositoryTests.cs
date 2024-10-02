using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
    }

    [Fact]
    public async Task Add_TaskComments_Success()
    {
        // Arrange
        const int count = 5;

        var taskComments = TaskCommentEntityV1Faker.Generate(count);

        // Act
        var results = await Task.WhenAll(taskComments.Select(x => _repository.Add(x, default)));

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(id => id > 0);
    }

    [Fact]
    public async Task Get_TaskComments_NotIncludeDeleted_Success()
    {
        // Arrange
        const int count = 5;

        var taskId = Random.Shared.NextInt64(1, 1_000_000_000);
        var taskComments = TaskCommentEntityV1Faker.Generate(count)
            .Select(tc => tc.WithTaskId(taskId))
            .ToArray();
        taskComments[^1] = taskComments[^1].WithDeletedAt(DateTimeOffset.UtcNow);

        var taskCommentIds = await Task.WhenAll(taskComments.Select(x => _repository.Add(x, default)));

        // Act
        var actualTaskComments = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = taskId,
            IncludeDeleted = false
        }, default);

        // Asserts
        var expectedTaskComments = taskComments
            .Zip(taskCommentIds, (tc, tcId) => tc with { Id = tcId })
            .Take(count - 1)
            .ToArray();

        actualTaskComments.Should().BeInDescendingOrder(tc => tc.At);
        actualTaskComments.Should().BeEquivalentTo(expectedTaskComments);
    }

    [Fact]
    public async Task Get_TaskComments_IncludeDeleted_Success()
    {
        // Arrange
        const int count = 5;

        var taskId = Random.Shared.NextInt64(1, 1_000_000_000);
        var taskComments = TaskCommentEntityV1Faker.Generate(count)
            .Select(tc => tc.WithTaskId(taskId))
            .ToArray();
        taskComments[^1] = taskComments[^1].WithDeletedAt(DateTimeOffset.UtcNow);

        var taskCommentIds = await Task.WhenAll(taskComments.Select(x => _repository.Add(x, default)));

        // Act
        var actualTaskComments = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = taskId,
            IncludeDeleted = true
        }, default);

        // Asserts
        var expectedTaskComments = taskComments
            .Zip(taskCommentIds, (tc, tcId) => tc with { Id = tcId })
            .ToArray();

        actualTaskComments.Should().BeInDescendingOrder(tc => tc.At);
        actualTaskComments.Should().BeEquivalentTo(expectedTaskComments);
    }

    [Fact]
    public async Task Modify_TaskComment_Success()
    {
        // Arrange
        var utcNow = DateTimeOffset.UtcNow;
        var message = utcNow.ToString("O");

        var taskId = Random.Shared.NextInt64(1, 1_000_000_000);
        var taskComment = TaskCommentEntityV1Faker.Generate()
            .Select(tc => tc.WithTaskId(taskId))
            .Single();

        var taskCommentId = await _repository.Add(taskComment, default);

        // Act
        await _repository.Update(taskComment with
            {
                Message = message,
                ModifiedAt = DateTime.UtcNow,
                Id = taskCommentId
            },
            default);

        // Assert

        var actualTaskComments = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = taskId,
            IncludeDeleted = false
        }, default);

        actualTaskComments.Should().HaveCount(1);
        actualTaskComments[0].Message.Should().Be(message);
        actualTaskComments[0].ModifiedAt.Should().BeAfter(utcNow);
    }

    [Fact]
    public async Task Delete_TaskComment_Success()
    {
        // Arrange
        var utcNow = DateTimeOffset.UtcNow;

        var taskId = Random.Shared.NextInt64(1, 1_000_000_000);
        var taskComment = TaskCommentEntityV1Faker.Generate()
            .Select(tc => tc.WithTaskId(taskId))
            .Single();

        var taskCommentId = (await _repository.Add(taskComment, default));

        // Act
        await _repository.SetDeleted(new TaskCommentDeleteModel
            {
                CommentId = taskCommentId,
                DeletedAt = DateTime.UtcNow
            },
            default);

        // Assert

        var actualTaskComments = await _repository.Get(new TaskCommentGetModel
        {
            TaskId = taskId,
            IncludeDeleted = true
        }, default);

        actualTaskComments.Should().HaveCount(1);
        actualTaskComments[0].DeletedAt.Should().BeAfter(utcNow);
    }
}