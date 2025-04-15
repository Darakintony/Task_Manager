using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.RegularExpressions;
using Task_Management.Model;
using Task_Management.Service;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTO;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Service
{
    public class UserAccountService
    {
        private readonly TaskManagementDbContext _context;
        private readonly IEmailService _emailService;

        public UserAccountService(TaskManagementDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        internal async Task<string?> UploadProfilePictureAsync(IFormFile? profilePicture)
        {
            if (profilePicture == null) return null; // No picture provided

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(profilePicture.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new Exception("Invalid image format. Allowed formats: .jpg, .jpeg, .png.");
            }

            // ✅ Ensure the directory exists
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/profile_pictures");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath); // 🔹 Create the directory if it doesn't exist
            }

            // ✅ Generate unique file name and save
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profilePicture.CopyToAsync(stream);
            }

            return $"/profile_pictures/{fileName}"; // Return the relative URL
        }


        internal async Task<Response<string>> GenerateEmailUpdateTokenAsync(Guid userId, string newEmail)
        {
            var user = await _context.UserMagTables.FindAsync(userId);
            if (user == null)
            {
                return new Response<string> { StatusCode = "96", StatusMessage = "User not found" };
            }

            if (string.IsNullOrEmpty(newEmail))
            {
                return new Response<string> { StatusCode = "96", StatusMessage = "New email is required" };
            }

            if (newEmail == user.Email)
            {
                return new Response<string> { StatusCode = "96", StatusMessage = "New email cannot be the same as the current email" };
            }

            // ✅ Generate a secure email confirmation token
            user.EmailConfirmationToken = WebEncoders.Base64UrlEncode(Guid.NewGuid().ToByteArray());
            user.EmailTokenExpiry = DateTime.UtcNow.AddMinutes(30); // Token valid for 30 minutes

            // ✅ Store new email temporarily
            user.PendingEmail = newEmail;

            await _context.SaveChangesAsync();

            // ✅ Send confirmation email to the **NEW** email
            string confirmationLink = $"https://localhost:7040/api/Users/confirm-email?token={user.EmailConfirmationToken}";
            await _emailService.SendEmailAsync(new Message(new string[] { newEmail }, "Confirm Your Email", confirmationLink));

            // ✅ Send notification to the **OLD** email (security measure)
            string securityAlert = "Your email update request was initiated. If this wasn't you, please contact support.";
            await _emailService.SendEmailAsync(new Message(new string[] { user.Email }, "Security Alert: Email Update", securityAlert));

            return new Response<string> { StatusCode = "00", StatusMessage = "Confirmation email sent to new email. Security alert sent to current email." };
        }

        //internal async Task<Response<string>> UpdateEmailAsync(Guid userId, UserUpdateRequest updateRequest)
        //internal async Task<Response<dynamic>> UpdateEmailAsync(Guid userId, string newEmail)
        //{
        //    var user = await _context.UserMagTables.FindAsync(userId);
        //    if (user == null)
        //    {
        //        return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
        //    }

        //    if (string.IsNullOrEmpty(newEmail))
        //    {
        //        return new Response<dynamic> { StatusCode = "96", StatusMessage = "New email is required" };
        //    }

        //    if (newEmail == user.Email)
        //    {
        //        return new Response<dynamic> { StatusCode = "96", StatusMessage = "New email cannot be the same as the current email" };
        //    }

        //    // ✅ Generate a secure email confirmation token
        //    user.EmailConfirmationToken = WebEncoders.Base64UrlEncode(Guid.NewGuid().ToByteArray());
        //    user.EmailTokenExpiry = DateTime.UtcNow.AddMinutes(30); // Token valid for 30 minutes

        //    // ✅ Store new email temporarily (won't replace current email until confirmed)
        //    user.PendingEmail = user.Email;

        //    await _context.SaveChangesAsync();

        //    // ✅ Send confirmation email to the **NEW** email
        //    string confirmationLink = $"https://localhost:7040/api/Users/confirm-email?token={user.EmailConfirmationToken}";
        //     _emailService.SendEmail(new Message(new string[] { user.Email }, "Confirm Your Email", confirmationLink));

        //    // ✅ Send notification to the **OLD** email (security measure)
        //    string securityAlert = "Your email update request was initiated. If this wasn't you, please contact support.";
        //    _emailService.SendEmail(new Message(new string[] { user.Email }, "Security Alert: Email Update", securityAlert));

        //    return new Response<dynamic> { StatusCode = "00", StatusMessage = "Confirmation email sent to new email. Security alert sent to current email." };
        //}


        internal async Task<Response<dynamic>> ForgotPasswordAsync(string email)
        {
            var user = await _context.UserMagTables.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
            }

            // Generate Reset Token
            user.PasswordResetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.PasswordTokenExpiry = DateTime.UtcNow.AddMinutes(15); // Token valid for 30 minutes

            await _context.SaveChangesAsync();

            // Send email with reset token
            //string resetLink = $"https://localhost:7040/api/Users/confirm-email?token={user.PasswordResetToken}";
            string resetLink = $"https://localhost:7040/api/users/reset-password?token={user.PasswordResetToken}";
            await _emailService.SendEmailAsync(new Message(new string[] { email }, "Reset Your Password", resetLink));

            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Password reset email sent successfully" };
        }

        public async Task<Response<dynamic>> ResetPasswordAsync(PasswordResetRequest request)
        {
            var user = await _context.UserMagTables.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
            }

            // Validate the reset token
            if (user.PasswordResetToken != request.ResetPasswordToken || user.PasswordTokenExpiry < DateTime.UtcNow)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Invalid or expired token" };
            }

            // Validate new password strength
            if (!IsValidPassword(request.NewPassword))
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character."
                };
            }

            // Update the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // ✅ Unlock account and reset failed login attempts
            user.IsAccountLocked = false;  // Unlock the account
            user.FailedLoginAttempts = 0;  // Reset failed login counter

            // Clear token after successful password reset
            user.PasswordResetToken = null;
            user.PasswordTokenExpiry = null;

            await _context.SaveChangesAsync();

            // Send confirmation email instead of reset link
            string subject = "Your Password Has Been Reset";
            string message = "Hello,\n\nYour password has been successfully changed. If you didn't make this change, please reset your password immediately or contact support.\n\nBest regards,\nYour Security Team";

            await _emailService.SendEmailAsync(new Message(new string[] { user.Email }, subject, message));

            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Password reset successfully. A confirmation email has been sent." };
        }

        internal async Task<Response<dynamic>> ChangePasswordAsync(Guid userId, ChangePasswordRequest updateRequest)
        {
        
            var user = await _context.UserMagTables.FindAsync(userId);
            if (user == null)
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "User not found" };
            }

            if (string.IsNullOrEmpty(updateRequest.CurrentPassword) || string.IsNullOrEmpty(updateRequest.NewPassword))
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Current password and new password are required" };
            }

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(updateRequest.CurrentPassword, user.Password))
            {
                return new Response<dynamic> { StatusCode = "96", StatusMessage = "Incorrect current password" };
            }

            if (!IsValidPassword(updateRequest.NewPassword))
            {
                return new Response<dynamic>
                {
                    StatusCode = "96",
                    StatusMessage = "Password must be at least 8 characters long, include an uppercase letter, a lowercase letter, a number, and a special character."
                };
            }
            // Update password
            user.Password = BCrypt.Net.BCrypt.HashPassword(updateRequest.NewPassword);
            await _context.SaveChangesAsync();
            string subject = "Your password has been changed";
            string message = "Hello,\n\nYour password has been successfully changed. If you didn't make this change, please reset your password immediately or contact support.\n\nBest regards,\nYour Security Team";

            await _emailService.SendEmailAsync(new Message(new string[] { user.Email }, subject, message));
            return new Response<dynamic> { StatusCode = "00", StatusMessage = "Password changed successfully" };
        }


        internal bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }
            //Improved Regex
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

    }
}
