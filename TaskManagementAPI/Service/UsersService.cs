using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public UsersService(TaskManagementDbContext context, ILogger<UsersService> logger,
            IConfiguration config)
        {
            _Context = context;
            _logger = logger;
            _configuration = config;
           
        }

        public async Task<Response<dynamic>> CreateUser(RegisterUser registerUser)
        {
            var User = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == registerUser.Email);
            if (User != null)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "This User has already exist"
                };
            }
            var newUser = new UserMagTable
            {
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Email = registerUser.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password)
              
            };
            
            await _Context.UserMagTables.AddAsync(newUser);
            _Context.SaveChanges();
            return new Response<dynamic>
            {
                StatusCode = "00",
                StatusMessage = "Success",
                Data = newUser.Id
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


