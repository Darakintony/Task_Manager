using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
        private readonly UserAccountService _userAccountService;
       
        public UsersController(IUsers UsersService, TaskManagementDbContext context, UserAccountService userAccountService) 
        { 
         _UsersService = UsersService;
            _Context = context;
            _userAccountService = userAccountService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterUser registerUser)
        {
            var response = await _UsersService.CreateUser(registerUser);

            return Ok(response); // Ensures response is serialized to JSON
        }

        [HttpGet("Get/User/Profile")]
         public async Task<IActionResult> GetUserProfile(Guid userId)
         {
            var user = await _UsersService.GetUserProfile(userId);
            return Ok(user);
         }

        [HttpPut("Update/User/Profile")]
        public async Task<IActionResult> UpdateUserProfile(Guid userId, [FromForm] UserUpdateRequest updateRequest)
        {
            if (updateRequest == null)
            {
                return BadRequest(new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Invalid request data"
                });
            }

            var response = await _UsersService.UpdateUserProfile(userId, updateRequest);
            return Ok(response);
        }

        [HttpGet("Confirm-Email")]
        public async Task<Response<dynamic>> ConfirmEmailAsync(string token)
        {
            var user = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.EmailConfirmationToken == token);

            if (user == null || user.EmailTokenExpiry < DateTime.UtcNow)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Invalid or expired token" };
            }

            if (!string.IsNullOrEmpty(user.PendingEmail))
            {
                // ✅ Email update process
                user.Email = user.PendingEmail;  // Move pending email to official email
                user.PendingEmail = null;        // Clear pending email field
            }

            user.IsEmailConfirmed = true;  // Mark email as confirmed
            user.EmailConfirmationToken = null;  // Remove token after confirmation
            user.EmailTokenExpiry = null;  // Remove token expiry

            await _Context.SaveChangesAsync();

            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Email successfully confirmed" };
        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(UserLogin loginRequest)
        {
            var token = await _UsersService.LoginUser(loginRequest);
            return Ok(token);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequest request)
        {
            var response = await _userAccountService.ResetPasswordAsync(request);
            return StatusCode(response.StatusCode == "00" ? 200 : 400, response);
        }

        [HttpPut("change-password")]
        public async Task<Response<dynamic>> ChangePasswordAsync([FromBody] ChangePasswordRequest updateRequest)
        {
            return await _userAccountService.ChangePasswordAsync(updateRequest.UserId, updateRequest);
        }
        [HttpPost("Forgot/Password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] FPasswordRequest request)
        {
            var response = await _userAccountService.ForgotPasswordAsync(request.Email);
            return Ok(response);
        }




    }
}
