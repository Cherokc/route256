using System;

namespace HomeworkApp.Bll.Models;

public record GetTaskCommentsModel
{
    public required long TaskId { get; init; }
    
    public required string Message { get; init; }
    
    public required bool IsDeleted { get; init; }
    
    public required DateTimeOffset At { get; init; }
}