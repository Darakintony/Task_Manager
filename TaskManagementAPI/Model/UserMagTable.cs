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
        public string Password { get; set; }
        public ICollection<ProjectMagTable> ProjectMagTables { get; set; }
    }
}
