namespace TaskManagementAPI.DTO
{
    public class TaskMagRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid ProjectId { get; set; }

    }
}
