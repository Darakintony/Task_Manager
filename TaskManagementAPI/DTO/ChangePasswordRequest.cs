using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTO
{
    public class ChangePasswordRequest
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }

        [MinLength(8, ErrorMessage = "New password must be at least 8 characters long")]
        public string NewPassword { get; set; }
    }
}
