namespace TaskManagementAPI.DTO
{
    public class ProjectRequest
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<UserTask> userTasks { get; set; }

        //public DateTime DueDate { get; set; }
        //public Enum Status { get; set; }
        //public Enum Priority { get; set; }

    }
}
