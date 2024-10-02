using System.Threading;
using System.Threading.Tasks;
using HomeworkApp.Bll.Models;

namespace HomeworkApp.Bll.Services.Interfaces;

public interface ITaskService
{
    Task<long> CreateTask(CreateTaskModel model, CancellationToken token);

    Task<GetTaskModel?> GetTask(long taskId, CancellationToken token);

    Task AssignTask(Bll.Models.AssignTaskModel model, CancellationToken token);
    
    Task<long> CreateComment(CreateTaskCommentModel model, CancellationToken token);
    
    Task<GetTaskCommentsModel[]> GetTaskComments(long taskId, CancellationToken token);
}