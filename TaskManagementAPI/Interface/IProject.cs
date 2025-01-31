using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Interface
{
    public interface IProject
    {
        public Task<List<Response2<dynamic>>> CreateProject(ProjectRequest projectRequest);
        public Task<Response2<dynamic>> GetAllProject(UsersProject project);
        public Task<Response2<dynamic>> GetProjectById(UsersProject projectId);
        public Task<Response2<dynamic>> UpdateProject(ProjectRequest projectRequest);
    }
}
