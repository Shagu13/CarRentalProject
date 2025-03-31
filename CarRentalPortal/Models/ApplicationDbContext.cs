using CarRentalPortal.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CarRentalPortal.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<RentalRecord> RentalRecords { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            // Store TransmissionType as string
            modelBuilder.Entity<Car>()
                .Property(c => c.TransmissionType)
                .HasConversion<string>();

            modelBuilder.Entity<Car>()
                .Property(c => c.EngineType)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Car>()
                .HasOne(c => c.User)
                .WithMany(u => u.Cars)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Serialize List<string> Images into JSON
            modelBuilder.Entity<Car>()
                .Property(c => c.Images)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
                );
            modelBuilder.Entity<RentalRecord>()
                .HasOne(r => r.User)
                .WithMany(u => u.Rentals)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<RentalRecord>()
                .HasOne(r => r.Car)
                .WithMany()
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
