using TaskManagementAPI.Enum;
namespace TaskManagementAPI.DTO
{
    public class TaskMagResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public Category Category { get; set; }
        public DateTime DeletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } 
        public Status Status { get; set; } 
        public Priority Priority { get; set; } 
        
    }


}

