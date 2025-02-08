﻿using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class ProjectService : IProject
    {
        public readonly TaskManagementDbContext _Context;
        private readonly ILogger<ProjectService> _logger;
        public ProjectService(TaskManagementDbContext Context, ILogger<ProjectService> logger)
        {
            _Context = Context;
            _logger = logger;
        }


        public async Task<Response2<dynamic>> CreateProject(ProjectRequest projectRequest)
        {
            if (string.IsNullOrWhiteSpace(projectRequest.UserId))
            {
                return new Response2<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "User id is required"
                };
            }


            if (!Guid.TryParse(projectRequest.UserId, out Guid userGuid))
            {
                return new Response2<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Invalid User ID format"
                };
            }

            var user = await _Context.UserMagTables.FindAsync(userGuid);
            if (user == null)
            {
                return new Response2<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Invalid User Id"
                };
            }

            var newproject = new ProjectMagTable
            {
                Title = projectRequest.Title,
                Description = projectRequest.Description,
                UsersId = userGuid
            };

            await _Context.ProjectMagTables.AddAsync(newproject);
            await _Context.SaveChangesAsync();

            return new Response2<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Project created successfully",
                Data = new { ProjectId = newproject.Id, Name = newproject.Title }
            };
        }

        public async Task<Response2<List<ProjectMagTable>>> GetProjectsByUserId(Guid userId)
        {
            try
            {
                var projects = await _Context.ProjectMagTables
                                             .Where(p => p.UsersId == userId)
                                             .ToListAsync();

                if (projects == null || !projects.Any())
                {
                    _logger.LogWarning("No projects found for User ID {UserId}", userId);
                    return new Response2<List<ProjectMagTable>>
                    {
                        StatusCode = "96",
                        StatusMessage = "No projects found for the specified user"
                    };
                }

                return new Response2<List<ProjectMagTable>>
                {
                    StatusCode = "00",
                    StatusMessage = "Success",
                    Data = projects
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving projects for User ID {UserId}", userId);
                return new Response2<List<ProjectMagTable>>
                {
                    StatusCode = "96",
                    StatusMessage = "An internal error occurred. Please try again later."
                };
            }
        }

        public async Task<Response2<ProjectMagTable>> GetProjectById(Guid id)
        {
            try
            {
                var project = await _Context.ProjectMagTables.FindAsync(id);

                if (project == null)
                {
                    _logger.LogWarning("Project with ID {ProjectId} not found", id);
                    return new Response2<ProjectMagTable>
                    {
                        StatusCode = "96",
                        StatusMessage = "Project not found"
                    };
                }

                return new Response2<ProjectMagTable>
                {
                    StatusCode = "00",
                    StatusMessage = "Success",
                    Data = project
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving project with ID {ProjectId}", id);
                return new Response2<ProjectMagTable>
                {
                    StatusCode = "96",
                    StatusMessage = "An internal error occurred. Please try again later."
                };
            }
        }


        public async Task<Response2<IEnumerable<ProjectResponse>>> GetAllProject()
        {
            try
            {
                var allProject = await _Context.ProjectMagTables.ToListAsync();


                if (!allProject.Any())
                {
                    _logger.LogInformation("No projects found in the database.");
                    return new Response2<IEnumerable<ProjectResponse>>
                    {
                        StatusCode = "96",
                        StatusMessage = "No record found",
                        Data = new List<ProjectResponse>()
                    };
                }


                var projectResponse = allProject.Select(x => new ProjectResponse(x.Title, x.Description, x.UsersId.ToString()));

                _logger.LogInformation("Successfully retrieved {ProjectCount} projects.", allProject.Count);

                return new Response2<IEnumerable<ProjectResponse>>
                {
                    StatusCode = "00",
                    StatusMessage = "You have successfully retrieved all projects",
                    Data = projectResponse
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving projects.");

                return new Response2<IEnumerable<ProjectResponse>>
                {
                    StatusCode = "99",
                    StatusMessage = "An internal error occurred. Please try again later.",
                    Data = new List<ProjectResponse>()
                };


            }
        }

       


      

      
    }
}
