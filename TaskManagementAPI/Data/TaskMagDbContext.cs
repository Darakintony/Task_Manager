using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Data
{

    public class TaskMagDbContext : DbContext
    {
        public TaskMagDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<UserTable> UserTables { get; set; }
        public DbSet<UsersProject> UsersProjects { get; set; }

    }
}
