namespace TaskManagementAPI.Model
{
    public class ProjectMagTable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid UsersId { get; set; }
        public UserMagTable UserMagTable { get; set; }
        public ICollection<TaskMagTable> TaskMagTables { get; set; }
    }
}
