using Microsoft.EntityFrameworkCore;

namespace Taskish.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Completed> Completed { get; set; }
        public DbSet<Deleted> Deleted { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = Taskish.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
           .HasMany(c => c.Tasks)
           .WithOne(t => t.Category)
           .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
