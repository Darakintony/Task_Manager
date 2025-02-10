using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class TaskMagService : ITaskMag
    {
        private readonly TaskManagementDbContext _Context;
        public TaskMagService(TaskManagementDbContext Context)
        {
            _Context = Context;
        }

        public async Task<Response<dynamic>> CreatTask(TaskMagRequest request)
        {

            if (request.ProjectId == Guid.Empty)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Project ID is required"
                };
            }
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Title is requird"
                };
            }
            var projectExist = await _Context.ProjectMagTables.AnyAsync(p => p.Id == request.ProjectId);
            if (!projectExist)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Associated project not found"
                };
            }

            var newTask = new TaskMagTable
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                ProjectId = request.ProjectId,
            };
            _Context.TaskMagTables.Add(newTask);
            await _Context.SaveChangesAsync();
            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Task created successfully",
                Data = newTask.Id
            };


        }

        public async Task<Response<List<TaskMagTable>>> GetTasksByProjectId(Guid projectId)
        {
            var tasks = await _Context.TaskMagTables.Where(tasks =>
            tasks.ProjectId == projectId).ToListAsync();

            if (tasks.Any())
            {
                return new Response<List<TaskMagTable>>
                {
                    StatusCode = "00",
                    StatusMessage = "success",
                    Data = tasks
                };
            }
            return new Response<List<TaskMagTable>>
            {
                StatusCode = "96",
                StatusMessage = "No task found for the specify project"
            };
                
        }
    }
}
