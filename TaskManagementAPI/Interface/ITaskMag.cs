
using Microsoft.VisualBasic;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Enum;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface ITaskMag
    {
       public Task<Response<dynamic>> CreateTask(TaskMagRequest request);
        public  Task<Response<List<TaskMagResponse>>> GetTasksByProjectId(Guid projectId);
        public Task<Response<dynamic>> UpdateTask(Guid projectId, Guid taskId, TaskMagUpdateRequest updateRequest);
        public Task<Response<dynamic>> DeleteTask(Guid projectId, Guid taskId);
        public  Task<Response<dynamic>> RestoreTask(Guid projectId, Guid taskId);
        public Task<Response<dynamic>> GetTaskById  (Guid taskId);
        Task<Response<List<TaskMagResponse>>> FilterTasks(Guid projectId, Status? status, Priority? priority, Category? category);
       
    }
}
