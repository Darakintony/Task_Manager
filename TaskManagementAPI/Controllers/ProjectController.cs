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

        //[Authorize]
        [HttpGet("GetAll-Project")]
        public async Task<IActionResult> GetAllProject()
        {
            var allProject = await _ProjectService.GetAllProject();
            return Ok(allProject);
        }


        //[Authorize]
        [HttpGet("Get-Project-By-Id")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            var project = await _ProjectService.GetProjectById(id);
            return Ok(project);
        }

        [HttpGet("Get_All_Project_For_A_User")]
        public async Task<IActionResult> GetProjectsByUserId(Guid userId)
        {

            var project = await _ProjectService.GetProjectsByUserId(userId);
            return Ok(project);
        }
    }
}
