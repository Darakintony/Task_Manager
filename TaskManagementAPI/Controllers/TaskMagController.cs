using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;

namespace TaskManagementAPI.Controllers
{
    [Authorize]
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

        [HttpPut("Update_Task/{projectId}/{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid projectId, Guid taskId, TaskMagUpdateRequest updateRequest)
        {
            var response = await _TaskMaskService.UpdateTask(projectId, taskId, updateRequest);
            return Ok(response);
        }


        [HttpDelete("Delete_Task")]
        public async Task<IActionResult> DeleteTask(Guid projectId,Guid taskId)
        {
            var response = await _TaskMaskService.DeleteTask(projectId,taskId);
            if (response.StatusCode == "00")
            {
                return Ok(response.StatusMessage);
            }
            else
            {
                return NotFound(response.StatusMessage);
            }
        }

        [HttpPut("{taskId}/restore")]
        public async Task<IActionResult> RestoreTask(Guid projectId, Guid taskId)
        {
            var result = await _TaskMaskService.RestoreTask(projectId, taskId);

            if (result.StatusCode == "00")
            {
                return Ok(result);
            }

            return NotFound(result);
        }


        [HttpPost("Get_Task_By_Id")]
        public async Task<IActionResult> GetTaskById(Guid taskId)
        {
            var task = await _TaskMaskService.GetTaskById(taskId);
            return Ok(task);
        }
        

    }
}
