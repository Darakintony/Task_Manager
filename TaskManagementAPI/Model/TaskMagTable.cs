using System.Text.Json.Serialization;

namespace TaskManagementAPI.Model
{
    public class TaskMagTable 
    {
      
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Title { get; set; }
            public string? Description { get; set; }
            public DateTime? DueDate { get; set; }
            public Guid ProjectId { get; set; }
            [JsonIgnore]
            public ProjectMagTable ProjectMagTable { get; set; }

        public Status Status { get; set; } = Status.Pending;
        public Priority Priority { get; set; } = Priority.Medium;
    }
    public enum Status
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3
    }

    public enum Priority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

}




