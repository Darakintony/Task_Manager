﻿using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class TaskMagService : ITaskMag
    {
        private readonly TaskManagementDbContext _Context;
        private readonly ILogger<TaskMagService> _Logger;
        public TaskMagService(TaskManagementDbContext Context, ILogger<TaskMagService> Logger)
        {
            _Context = Context;
            _Logger = Logger;
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
           
            var datee = ReformedDate(request.DueDate.ToString());
            if (datee == "96")
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Invalid date format. Use dd/MM/yyyy, yyyy/MM/dd, dd-MM-yyyy, MM-dd-yyyy, MM/dd/yyyy, yyyy-MM-dd"
                };
            }
            var newTask = new TaskMagTable
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = Convert.ToDateTime(datee), // (DateTime)d,/* request.DueDate.Value.Date,// .toString(17-02-2000)*/
                CreatedAt = DateTime.UtcNow,
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

        public async Task<Response<dynamic>> UpdateTask(Guid projectId,Guid taskId, TaskMagUpdateRequest updateRequest)
        {
            try
            {
                var task = await _Context.TaskMagTables.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
                if (task == null)
                {
                    _Logger.LogWarning("Task with the Id {TaskId} not found in project{ProjectId}", taskId, projectId);
                    return new Response<dynamic>
                    {
                        StatusCode = "96",
                        StatusMessage = "Task not found in the specify project"
                    };
                }
                task.Title = updateRequest.Title ?? task.Title;
                task.Description = updateRequest.Description ?? task.Description;
                task.DueDate = updateRequest.DueDate ?? task.DueDate;
                task.Status = updateRequest.Status ?? task.Status;
                task.Priority = updateRequest.Priority ?? task.Priority;

                _Context.TaskMagTables.Update(task);
                await _Context.SaveChangesAsync();

                _Logger.LogInformation("Task with Id {TaskId} updated successfully in uproject {projectId}", taskId, projectId);
                return new Response<dynamic>
                {
                    StatusCode = "00",
                    StatusMessage = "Task updated successfully",
                    Data = task
                };
            }
            catch (Exception ex) 
            { 
             _Logger.LogError("An error occur while updating task with Id {taskId} in project {projectId}", taskId, projectId);
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "An error occur while updating the task"
                };
            };
           

        }

        public async Task<Response<dynamic>> DeleteTask(Guid projectId, Guid taskId)
        {
            var task = await _Context.TaskMagTables.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
            if (task == null || task.IsDeleted)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = " Task not found"
                };
            }

            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            _Context.TaskMagTables.Update(task);
            await _Context.SaveChangesAsync();
            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Task deleted successfully"
            };
        }

        public async Task<Response<dynamic>> RestoreTask(Guid projectId, Guid taskId)
        {
            _Logger.LogInformation("Restoring task: TaskId={TaskId}, ProjectId={ProjectId}", taskId, projectId);

            var task = await _Context.TaskMagTables
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId && t.IsDeleted);

            if (task == null)
            {
               // _Logger.LogWarning("Task not found or not deleted: TaskId={TaskId}, ProjectId={ProjectId}", taskId, projectId);
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Task not found or not deleted"
                };
            }

            task.IsDeleted = false;
            task.DeletedAt = null; // Clear the deletion timestamp
            _Context.TaskMagTables.Update(task);
            await _Context.SaveChangesAsync();

            _Logger.LogInformation("Task restored successfully: TaskId={TaskId}, ProjectId={ProjectId}", taskId, projectId);
            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Task restored successfully",
                Data = task
            };
        }

        public async Task<Response<dynamic>> GetTaskById(Guid taskId)
        {
            var task = await _Context.TaskMagTables.FindAsync(taskId);
            if (task == null)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Invalid Id"
                };
            }
            task.IsDeleted = false;
            task.DeletedAt = null;
            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Task retrieved successfull",
                Data = task
            };
        }

        private string ReformedDate(string DueDate)
        {
            string[] formats = { "yyyy-MM-dd", "MM/dd/yyyy", "MM-dd-yyyy", "dd-MM-yyyy", "yyyy/MM/dd", "dd/MM/yyyy" };
            if (DateTime.TryParseExact(DueDate, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.ToString("yyyy-MM-dd");
            }
            return "96";
        }

    }
}
