using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagementAPI.Model
{
    public class ProjectMagTable
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid UsersId { get; set; }
        [JsonIgnore]
        public UserMagTable UserMagTable { get; set; }
        [JsonIgnore]
        public ICollection<TaskMagTable> TaskMagTables { get; set; }
    }
}
