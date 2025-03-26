using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;
using TaskManagementAPI.Service;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public readonly IUsers _UsersService;
        private readonly TaskManagementDbContext _Context;
       
        public UsersController(IUsers UsersService, TaskManagementDbContext context) 
        { 
         _UsersService = UsersService;
            _Context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUser registerUser)
        {
            var response = await _UsersService.CreateUser(registerUser);

            return Ok(response); // Ensures response is serialized to JSON
        }

        //[HttpPost("Register")]
        //public async Task<IActionResult> CreateUser(RegisterUser registerUser)
        //{
        //    var newUser = await _UsersService.CreateUser(registerUser);
        //    return Ok(newUser);
        //}

        [HttpGet("Get/User/Profile")]
         public async Task<IActionResult> GetUserProfile(Guid userId)
         {
            var user = await _UsersService.GetUserProfile(userId);
            return Ok(user);
         }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == email && u.EmailConfirmationToken == token);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response<dynamic> { StatusCode = "96", StatusMessage = "Invalid token or email." });
            }

            // ✅ Update email confirmation status
            user.IsEmailConfirmed = true; // Set to TRUE
            user.EmailConfirmationToken = null; // Clear the token

            _Context.UserMagTables.Update(user);
            await _Context.SaveChangesAsync(); // ✅ Updates the database

            return StatusCode(StatusCodes.Status200OK,
                new Response<dynamic> { StatusCode = "00", StatusMessage = "Email verified successfully!" });
        }




        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin model) 
        {
            var token = await _UsersService.Login(model);
            return Ok(token);
        }

    }
}
