using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.DTO
{
    public class FPasswordRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
