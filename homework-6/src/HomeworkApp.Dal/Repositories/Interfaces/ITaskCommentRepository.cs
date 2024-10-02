using System.Threading;
using System.Threading.Tasks;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;

namespace HomeworkApp.Dal.Repositories.Interfaces;

public interface ITaskCommentRepository
{
    Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token);
    
    Task<long> Add(TaskCommentEntityV1 model, CancellationToken token);
    
    Task Update(TaskCommentEntityV1 model, CancellationToken token);
    
    Task SetDeleted(TaskCommentDeleteModel model, CancellationToken token);
    
}