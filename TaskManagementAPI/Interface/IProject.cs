using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IProject
    {
        public Task<Response2<dynamic>> CreateProject(ProjectRequest projectRequest);
        public Task<Response2<IEnumerable<ProjectResponse>>> GetAllProject();
        public Task<Response2<ProjectMagTable>> GetProjectById(Guid id);
        Task<Response2<List<ProjectMagTable>>> GetProjectsByUserId(Guid userId);
        public Task<Response> DeleteProject(Guid id);
        Task<Response2<dynamic>> UpdateProject(Guid projectId, TaskMagProjectUpdate updateRequest);
      


    }
}
