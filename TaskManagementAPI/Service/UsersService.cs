
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Task_Management.Model;
using Task_Management.Service;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Interface;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class UsersService : IUsers
    {
        private readonly TaskManagementDbContext _Context;
        private ILogger<UsersService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UsersService(TaskManagementDbContext context, ILogger<UsersService> logger,
            IConfiguration config, IEmailService emailService)
        {
            _Context = context;
            _logger = logger;
            _configuration = config;
            _emailService = emailService;
           
        }
        public async Task<Response<dynamic>> CreateUser(RegisterUser registerUser)
        {
            var existingUser = await _Context.UserMagTables
                .FirstOrDefaultAsync(u => u.Email == registerUser.Email);

            if (existingUser != null)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "This User already exists"
                };
            }

            // ✅ Ensure profile_pictures directory exists
            var profilePicturesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profile_pictures");
            if (!Directory.Exists(profilePicturesPath))
            {
                Directory.CreateDirectory(profilePicturesPath);
            }

            // ✅ Handle profile picture upload
            string? profilePictureUrl = null;
            if (registerUser.ProfilePicture != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(registerUser.ProfilePicture.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new Response<dynamic> { StatusCode = "96", StatusMessage = "Invalid image format. Allowed formats: .jpg, .jpeg, .png." };
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(profilePicturesPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await registerUser.ProfilePicture.CopyToAsync(stream);
                }

                string baseUrl = "http://localhost:5260"; // Replace with your actual domain
                profilePictureUrl = $"{baseUrl}/profile_pictures/{fileName}";
            }

            // ✅ Create new user
            var newUser = new UserMagTable
            {
                Id = Guid.NewGuid(),
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password),
                IsEmailConfirmed = false,
                ProfilePicture = profilePictureUrl,
                PhoneNumber = registerUser.PhoneNumber,
                TokenExpiry = DateTime.UtcNow.AddMinutes(20),
                EmailConfirmationToken = WebEncoders.Base64UrlEncode(Guid.NewGuid().ToByteArray()) // ✅ URL-safe token
            };

            await _Context.UserMagTables.AddAsync(newUser);
            await _Context.SaveChangesAsync();

           

            // ✅ Send confirmation email asynchronously
            string confirmationLink = $"https://localhost:7040/api/Users/confirm-email?token={newUser.EmailConfirmationToken}&email={registerUser.Email}";
            string emailBody = $"Click the link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>";

            _emailService.SendEmail(new Message(new string[] { registerUser.Email }, "Confirm your Email", emailBody));

            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Success! Check your email to confirm your account.",
                Data = new
                {
                    UserId = newUser.Id,
                    newUser.Email,
                    ProfilePictureUrl = newUser.ProfilePicture
                }
            };
        }


        public async Task<Response<dynamic>> GetUserProfile(Guid userId)
        {
            var user = await _Context.UserMagTables.FindAsync(userId);
            if (user == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found." };
            }

            return new Response<dynamic>
            {
                StatusCode = "00",
                Data = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    ProfilePicture = user.ProfilePicture // ✅ Return image URL
                }
            };
        }


        public async Task<Response<dynamic>> Login(UserLogin request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return new Response<dynamic>
                    {
                        StatusCode = "96",
                        StatusMessage = "Username and Password are required"
                    };
                }

                var user = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                {
                    return new Response<dynamic>
                    {
                        StatusCode = "96",
                        StatusMessage = "Invalid details"
                    };
                }

                // 🔹 Ensure email is confirmed before allowing login
                if (!user.IsEmailConfirmed)
                {
                    return new Response<dynamic>
                    {
                        StatusCode = "97",
                        StatusMessage = "Email is not confirmed. Please verify your email."
                    };
                }
                // Generate JWT Token
                var token = GenerateJwtToken(user);

                return new Response<dynamic>
                {
                    StatusCode = "00",
                    Data = new { Token = token }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login");
                return new Response<dynamic>
                {
                    StatusCode = "99",
                    StatusMessage = "An error occurred during login"
                };
            }
        }

        private string GenerateJwtToken(UserMagTable user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString())
                // Add additional claims as needed
            }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}


