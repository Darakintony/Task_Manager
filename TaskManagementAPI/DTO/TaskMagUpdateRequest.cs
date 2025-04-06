using System.Text.Json.Serialization;
using TaskManagementAPI.Enum;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.DTO
{
    public class TaskMagUpdateRequest 
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Status? Status { get; set; }         // Enum made nullable
        public Priority? Priority { get; set; }
        public string? Category { get; set; }
       
    }
}
