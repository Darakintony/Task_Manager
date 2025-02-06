using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;
using TaskManagementAPI.Service;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public readonly IProject _ProjectService;
        public ProjectController(IProject projectService)
        {
            _ProjectService = projectService;
        }
        [HttpPost("Create Project")]
        public async Task<IActionResult> CreateProject(ProjectRequest projectRequest)
        {
            var newProject = await _ProjectService.CreateProject(projectRequest);
            return Ok(newProject);
        }

       // [Authorize]
        [HttpGet("GetAll Project")]
        public async Task<IActionResult> GetAllProject()
        {
            var allProject = await _ProjectService.GetAllProject();
            return Ok(allProject);
        }

    }
}
