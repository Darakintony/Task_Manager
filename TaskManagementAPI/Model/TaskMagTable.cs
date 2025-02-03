namespace TaskManagementAPI.Model
{
    public class TaskMagTable 
    {
      
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime DueDate { get; set; }
            public Guid ProjectId { get; set; }
            public ProjectMagTable ProjectMagTable { get; set; }
    }
        public enum TaskStatus { Pending = 1, InProgress = 2, Completed = 3 }

        public enum Priority { High = 1, Low = 2, Medium }

    
}




