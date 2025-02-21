using System.Text.Json.Serialization;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.DTO
{
    public class TaskMagUpdateRequest
    {
        //public Guid Id { get; set; } 
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
       // public Guid ProjectId { get; set; }
        public Status? Status { get; set; } //= Status.Pending;
        public Priority? Priority { get; set; } //= Priority.Medium;
    }
}
