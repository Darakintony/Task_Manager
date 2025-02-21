using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;
using TaskManagementAPI.Service;

namespace TaskManagementAPI.Controllers
{
   [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        public readonly IProject _ProjectService;
        public ProjectController(IProject projectService)
        {
            _ProjectService = projectService;
        }
        [HttpPost("Create_Project")]
        public async Task<IActionResult> CreateProject(ProjectRequest projectRequest)
        {
            var newProject = await _ProjectService.CreateProject(projectRequest);
            return Ok(newProject);
        }

        
        [HttpGet("GetAll_Project")]
        public async Task<IActionResult> GetAllProject()
        {
            var allProject = await _ProjectService.GetAllProject();
            return Ok(allProject);
        }


        
        [HttpPost("Get_Project_By_Id")]
        public async Task<IActionResult> GetProjectById([FromBody]Guid id)
        {
            var project = await _ProjectService.GetProjectById(id);
            return Ok(project);
        }

        [HttpGet("Get_All_Project_For_A_User")]
        public async Task<IActionResult> GetProjectsByUserId([FromBody]Guid userId)
        {

            var project = await _ProjectService.GetProjectsByUserId(userId);
            return Ok(project);
        }

        [HttpDelete("Delete_Project")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _ProjectService.DeleteProject(id);
            return Ok(project);
        }


        [HttpPut("Update_Project{projectId}")]
        public async Task<IActionResult> UpdateProject(Guid projectId, [FromBody] ProjectUpdateRequest updateRequest)
        {
            var response = await _ProjectService.UpdateProject(projectId, updateRequest);
            if (response.StatusCode == "00")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
