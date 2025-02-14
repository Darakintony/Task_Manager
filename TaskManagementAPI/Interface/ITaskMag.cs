using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface ITaskMag
    {
       public Task<Response<dynamic>> CreatTask(TaskMagRequest request);
        public  Task<Response<List<TaskMagTable>>> GetTasksByProjectId(Guid projectId);
        public Task<Response<dynamic>> UpdateTask(Guid projectId, Guid taskId, TaskMagUpdateRequest updateRequest);
       
    }
}
