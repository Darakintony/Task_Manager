using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<UserMagTable> UserMagTables { get; set; }
        public DbSet<ProjectMagTable> ProjectMagTables { get; set; }
        public DbSet<TaskMagTable> TaskMagTables { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserMagTable>().HasMany(u => u.ProjectMagTables).WithOne(p =>
            p.UserMagTable).HasForeignKey(p => p.UsersId).HasPrincipalKey(u => u.Id);

            modelBuilder.Entity<ProjectMagTable>().HasMany(p => p.TaskMagTables).WithOne(t =>
            t.ProjectMagTable).HasForeignKey(t => t.ProjectId);
        }
    }
}
