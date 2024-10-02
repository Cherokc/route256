using System;

namespace HomeworkApp.Dal.Models;

public record TaskCommentDeleteModel
{
    public required long CommentId { get; init; }
    
    public required DateTimeOffset DeletedAt { get; init; }
}