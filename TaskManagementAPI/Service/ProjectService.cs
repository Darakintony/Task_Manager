using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class ProjectService : IProject
    {
        public readonly TaskMagDbContext _Context;
        public ProjectService(TaskMagDbContext Context)
        {
            _Context = Context;
        }
            
        public async Task<List<Response2<dynamic>>> CreateProject(ProjectRequest projectRequest)
        {
            var project = new UsersProject
            {
                Title = projectRequest.Title,
                Description = projectRequest.Description,
                
            };
            
            
        }

        public Task<Response2<dynamic>> GetAllProject(UsersProject project)
        {
            throw new NotImplementedException();
        }

        public Task<Response2<dynamic>> GetProjectById(UsersProject projectId)
        {
            throw new NotImplementedException();
        }

        public Task<Response2<dynamic>> UpdateProject(ProjectRequest projectRequest)
        {
            throw new NotImplementedException();
        }
    }
}
