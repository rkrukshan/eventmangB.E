using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventBooking> EventBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired().HasMaxLength(10);
                entity.Property(u => u.CreatedAt).IsRequired();

                // Index for better performance
                entity.HasIndex(u => u.Username).IsUnique();
            });

            // Event configuration
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);

                // Ensure end date is after start date
                entity.HasCheckConstraint("CK_Event_DateRange", "EndDate >= StartDate");
            });

            // EventBooking configuration
            modelBuilder.Entity<EventBooking>(entity =>
            {
                entity.HasKey(eb => eb.Id);
                entity.Property(eb => eb.EventName).IsRequired().HasMaxLength(200);
                entity.Property(eb => eb.UserName).IsRequired().HasMaxLength(100);
                entity.Property(eb => eb.BookingDate).IsRequired();

                // Relationships
                entity.HasOne(eb => eb.Event)
                    .WithMany(e => e.Bookings)
                    .HasForeignKey(eb => eb.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(eb => eb.User)
                    .WithMany()
                    .HasForeignKey(eb => eb.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate bookings
                entity.HasIndex(eb => new { eb.EventId, eb.UserId }).IsUnique();
            });

            // Seed data
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "admin", CreatedAt = DateTime.UtcNow },
                new User { Id = 2, Username = "John Doe", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), Role = "user", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Tech Conference 2024",
                    Description = "Annual technology conference",
                    StartDate = DateTime.Now.AddDays(30),
                    EndDate = DateTime.Now.AddDays(32)
                },
                new Event
                {
                    Id = 2,
                    Name = "Music Festival",
                    Description = "Summer music festival",
                    StartDate = DateTime.Now.AddDays(45),
                    EndDate = DateTime.Now.AddDays(47)
                }
            );
        }
    }
}