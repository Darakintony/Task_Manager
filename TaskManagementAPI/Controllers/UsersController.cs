using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IUsers _UsersService;
        public UsersController(IUsers UsersService) 
        { 
         _UsersService = UsersService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser(RegisterUser registerUser)
        {
            var newUser = await _UsersService.CreateUser(registerUser);
            return Ok(newUser);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin request) 
        {
            var user = await _UsersService.Login(request);
            return Ok(user);
        }

    }
}
