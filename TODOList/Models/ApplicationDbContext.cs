using Microsoft.EntityFrameworkCore;
using TODOList.Entities;

namespace TODOList.Models
    {
    public class ApplicationDbContext : DbContext
        {
        public DbSet<User> User { get; set; }
        public DbSet<TodoItem> TodoItem { get; set; }

        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base (options)
            {
            }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
            {
            base.OnModelCreating (modelBuilder);

            modelBuilder.Entity<TodoItem> (entity =>
                {
                entity.HasKey (e => e.Id);
                entity.Property (e => e.Name).IsRequired ();
                entity.Property (e => e.IsDone).HasDefaultValue (false);
                entity.HasOne (d => d.User)
                    .WithMany (p => p.Todos);
                });

            modelBuilder.Entity<User> (entity =>
                {
                entity.HasKey (e => e.Id);
                entity.Property (e => e.Email).IsRequired ();
                entity.Property (e => e.Role);
                });
            }
        }
    }
