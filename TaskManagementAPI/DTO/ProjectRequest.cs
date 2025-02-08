using System.Reflection;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.DTO
{
    public class ProjectRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; }
              
    }

    public record ProjectResponse (string Title, string Description, string UserId);
    
}
