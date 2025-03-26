using TaskManagementAPI.Model;

namespace TaskManagementAPI.DTO
{
    public class TaskMagRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? DueDate { get; set; }
        public Guid ProjectId { get; set; }
        public TaskCategory Category { get; set; }

    }
}
