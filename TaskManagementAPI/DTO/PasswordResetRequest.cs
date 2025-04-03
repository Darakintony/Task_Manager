using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTO
{
    public class PasswordResetRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Reset password token is required")]
          public string ResetPasswordToken { get; set; }

         [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
         public string NewPassword { get; set; }

    }
}
