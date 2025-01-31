namespace TaskManagementAPI.Model
{
    public class UsersProject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public Enum Status { get; set; }
        public Enum Priority { get; set; }
        public int UserId { get; set; }
    }
}
