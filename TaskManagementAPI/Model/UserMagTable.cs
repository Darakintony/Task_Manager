using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Model
{
    public class UserMagTable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();  // Auto-generate GUID
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PendingEmail { get; set; }
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;  // Default false
        public string EmailConfirmationToken { get; set; }   // Store token
        public DateTime? EmailTokenExpiry { get; set; } // ✅ Token Expiration Time
        public int FailedLoginAttempts { get; set; } = 0; // Track failed attempts
        public bool IsAccountLocked { get; set; } = false; // Lock account if needed
        public string? PasswordResetToken { get; set; } // ✅ Reset Token
        public DateTime? PasswordTokenExpiry { get; set; } // Expiration Time for Token
        public ICollection<ProjectMagTable> ProjectMagTables { get; set; }
    }
}
