using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManagementAPI.Model;

namespace TaskManagementAPI.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options) : base(options)
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

            //var taskStatusConverter = new EnumToStringConverter<Status>();
            modelBuilder
                .Entity<TaskMagTable>()
                .Property(e => e.Status)
                .HasConversion<string>();

            // Configure Priority enum to be stored as string
            //var priorityConverter = new EnumToStringConverter<Priority>();
            modelBuilder
                .Entity<TaskMagTable>()
                .Property(e => e.Priority)
                .HasConversion<string>();

            // To exclude soft deleted Task authomatically
            modelBuilder
                .Entity<TaskMagTable>()
                .HasQueryFilter(t => !t.IsDeleted);

        }
    }
}
