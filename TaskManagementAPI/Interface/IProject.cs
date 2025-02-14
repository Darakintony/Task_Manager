using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IProject
    {
        public Task<Response<dynamic>> CreateProject(ProjectRequest projectRequest);
        public Task<Response<IEnumerable<ProjectResponse>>> GetAllProject();
        public Task<dynamic> GetProjectById(Guid id);
        Task<Response<List<ProjectMagTable>>> GetProjectsByUserId(Guid userId);
        public Task<Response<bool>> DeleteProject(Guid id);
        Task<Response<dynamic>> UpdateProject(Guid Id, ProjectUpdateRequest updateRequest);
      


    }
}
