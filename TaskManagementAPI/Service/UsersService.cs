
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly UserAccountService _userAccountService;

        public UsersService(TaskManagementDbContext context, ILogger<UsersService> logger,
            IConfiguration config, IEmailService emailService, UserAccountService userAccountService)
        {
            _Context = context;
            _logger = logger;
            _configuration = config;
            _emailService = emailService;
            _userAccountService = userAccountService;
        }

        public async Task<Response<dynamic>> CreateUser(RegisterUser registerUser)
        {
            // ✅ Trim email to avoid accidental spaces
            registerUser.Email = registerUser.Email.Trim();

            // ✅ Validate Password Strength First
            if (!_userAccountService.IsValidPassword(registerUser.Password))
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character."
                };
            }

            // ✅ Check if the email is already registered
            var existingUser = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == registerUser.Email);
            if (existingUser != null)
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "This User already exists"
                };
            }

            // ✅ Upload Profile Picture
            var profilePictureUrl = await _userAccountService.UploadProfilePictureAsync(registerUser.ProfilePicture);

            // ✅ Create New User
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
                EmailTokenExpiry = DateTime.UtcNow.AddHours(6),
                EmailConfirmationToken = WebEncoders.Base64UrlEncode(Guid.NewGuid().ToByteArray()) // ✅ URL-safe token
            };

            await _Context.UserMagTables.AddAsync(newUser);
            await _Context.SaveChangesAsync();

            // ✅ Send Confirmation Email
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

        //public async Task<Response<dynamic>> CreateUser(RegisterUser registerUser)
        //{
        //    var existingUser = await _Context.UserMagTables
        //        .FirstOrDefaultAsync(u => u.Email == registerUser.Email);

        //    if (existingUser != null)
        //    {
        //        return new Response<dynamic>
        //        {
        //            StatusCode = "96",
        //            StatusMessage = "This User already exists"
        //        };
        //    }
        //    if (!IsValidPassword(registerUser.Password))
        //    {
        //        return new Response<dynamic>
        //        {
        //            StatusCode = "96",
        //            StatusMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character."
        //        };
        //    }

        //    var profilePictureUrl = await _userAccountService.UploadProfilePictureAsync(registerUser.ProfilePicture);
        //    //var emailResponse = await _userAccountService.GenerateEmailUpdateTokenAsync(newUser.Id, newUser.Email);

        //    // ✅ Create new user
        //    var newUser = new UserMagTable
        //    {
        //        Id = Guid.NewGuid(),
        //        FirstName = registerUser.FirstName,
        //        LastName = registerUser.LastName,
        //        Email = registerUser.Email,
        //        Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password),
        //        IsEmailConfirmed = false,
        //        ProfilePicture = profilePictureUrl,
        //        PhoneNumber = registerUser.PhoneNumber,
        //        EmailTokenExpiry = DateTime.UtcNow.AddHours(6),
        //        EmailConfirmationToken = WebEncoders.Base64UrlEncode(Guid.NewGuid().ToByteArray()) // ✅ URL-safe token
        //    };

        //    await _Context.UserMagTables.AddAsync(newUser);
        //    await _Context.SaveChangesAsync();

        //    // ✅ Send confirmation email asynchronously
        //    string confirmationLink = $"https://localhost:7040/api/Users/confirm-email?token={newUser.EmailConfirmationToken}&email={registerUser.Email}";
        //    string emailBody = $"Click the link to confirm your email: <a href='{confirmationLink}'>Confirm Email</a>";

        //    _emailService.SendEmail(new Message(new string[] { registerUser.Email }, "Confirm your Email", emailBody));

        //    return new Response<dynamic>
        //    {
        //        StatusCode = "00",
        //        StatusMessage = "Success! Check your email to confirm your account.",
        //        Data = new
        //        {
        //            UserId = newUser.Id,
        //            newUser.Email,
        //            ProfilePictureUrl = newUser.ProfilePicture
        //        }
        //    };
        //}


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

        public async Task<Response<dynamic>> LoginUser(UserLogin loginRequest)
        {
            loginRequest.Email = loginRequest.Email.Trim();
            var user = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == loginRequest.Email);
            if (user == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
            }
            //if (loginRequest.Password != user.Password)
            //{
            //    return new Response<dynamic>
            //    {
            //        StatusCode = "96",
            //        StatusMessage = "Invalid password"
            //    };
            //}

            // Check if the account is locked
            if (user.IsAccountLocked)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Account is locked due to multiple failed attempts. Reset your password to unlock." };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password))
            {
                user.FailedLoginAttempts++; // Increase failed attempts

                if (user.FailedLoginAttempts >= 4)
                {
                    user.IsAccountLocked = true; // Lock account after 4 failed attempts
                }

                await _Context.SaveChangesAsync();

                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Incorrect password" };
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            await _Context.SaveChangesAsync();
            //Generate JWT Token
           var token = GenerateJwtToken(user);


            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Login successful", Data = token };
        }

        //public async Task<Response<dynamic>> Login(UserLogin request)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        //        {
        //            return new Response<dynamic>
        //            {
        //                StatusCode = "96",
        //                StatusMessage = "Username and Password are required"
        //            };
        //        }

        //        var user = await _Context.UserMagTables.FirstOrDefaultAsync(u => u.Email == request.Email);
        //        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        //        {
        //            return new Response<dynamic>
        //            {
        //                StatusCode = "96",
        //                StatusMessage = "Invalid details"
        //            };
        //        }

        //        // 🔹 Ensure email is confirmed before allowing login
        //        if (!user.IsEmailConfirmed)
        //        {
        //            return new Response<dynamic>
        //            {
        //                StatusCode = "97",
        //                StatusMessage = "Email is not confirmed. Please verify your email."
        //            };
        //        }
        //        // Generate JWT Token
        //        var token = GenerateJwtToken(user);

        //        return new Response<dynamic>
        //        {
        //            StatusCode = "00",
        //            Data = new { Token = token }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred during login");
        //        return new Response<dynamic>
        //        {
        //            StatusCode = "99",
        //            StatusMessage = "An error occurred during login"
        //        };
        //    }
        //}

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

        public async Task<Response<dynamic>> UpdateUserProfile(Guid userId, UserUpdateRequest updateRequest)
        {
            var userExist = await _Context.UserMagTables.FindAsync(userId);
            if (userExist == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
            }

            // ✅ Handle email update
            if (!string.IsNullOrEmpty(updateRequest.Email) && updateRequest.Email != userExist.Email)
            {
                var emailResponse = await _userAccountService.GenerateEmailUpdateTokenAsync(userId, updateRequest.Email);
                if (emailResponse.StatusCode != "00")
                {
                    return new Response<dynamic> { StatusCode = "96", StatusMessage = "Failed to send confirmation email" };
                }

                return new Response<dynamic> { StatusCode = "97", StatusMessage = "Email update requires verification" };
            }

            // ✅ Handle profile picture upload
            if (updateRequest.ProfilePicture != null)
            {
                string? newProfilePicture = await _userAccountService.UploadProfilePictureAsync(updateRequest.ProfilePicture);
                if (!string.IsNullOrEmpty(newProfilePicture))
                {
                    userExist.ProfilePicture = newProfilePicture;
                }
            }

            // ✅ Handle password change(User - provided current and new password)
            //if (!string.IsNullOrEmpty(updateRequest.CurrentPassword) && !string.IsNullOrEmpty(updateRequest.NewPassword))
            //{
            //    var passwordResponse = await _userAccountService.ChangePasswordAsync(userId, updateRequest);
            //    if (passwordResponse.StatusCode != "00")
            //    {
            //        return passwordResponse; // Return error if password change fails
            //    }
            //}

            // ✅ Update other profile details if provided
            userExist.FirstName = updateRequest.FirstName ?? userExist.FirstName;
            userExist.LastName = updateRequest.LastName ?? userExist.LastName;
            userExist.PhoneNumber = updateRequest.PhoneNumber ?? userExist.PhoneNumber;

            await _Context.SaveChangesAsync();

            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Profile updated successfully" };
        }
    
        
    }


}


