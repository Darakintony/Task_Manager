using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskMagController : ControllerBase
    {
        public readonly ITaskMag _TaskMaskService;
        public TaskMagController(ITaskMag TaskMaskService)
        {
            _TaskMaskService = TaskMaskService;
        }

        [HttpPost("Create_Task")]
        public async Task<IActionResult> CreateTask (TaskMagRequest request)
        {
            var task = await _TaskMaskService.CreatTask(request);
            return Ok(task);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProjectId(Guid projectId)
        {
            var response = await _TaskMaskService.GetTasksByProjectId(projectId);

            if (response.StatusCode == "00")
            {
                return Ok(response.Data);
            }
            else
            {
                return NotFound(response.StatusMessage);
            }
        }
    }
}
