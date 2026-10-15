
using API_proj.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_proj.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(
            DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Enrolments> Enrolments { get; set; }
        public DbSet<Models.Results> Results { get; set; }
        public DbSet<Routes> Routes { get; set; }
        public DbSet<Weather> Weather { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Each event belongs to one organiser.
            modelBuilder.Entity<Events>()
                .HasOne(e => e.Organiser)
                .WithMany()
                .HasForeignKey(e => e.OrganiserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Each category belongs to one event.
            modelBuilder.Entity<Categories>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category names must be unique within each event.
            modelBuilder.Entity<Categories>()
                .HasIndex(c => new { c.EventId, c.CategoryName })
                .IsUnique();

            // Each enrolment references a participant, event and category.
            modelBuilder.Entity<Enrolments>()
                .HasOne(e => e.Participant)
                .WithMany()
                .HasForeignKey(e => e.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolments>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrolments>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Only one result can exist per enrolment.
            modelBuilder.Entity<Models.Results>()
                .HasIndex(r => r.EnrolmentId)
                .IsUnique();

            modelBuilder.Entity<Models.Results>()
                .HasOne(r => r.Enrolment)
                .WithMany()
                .HasForeignKey(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Routes belong to events.
            modelBuilder.Entity<Routes>()
                .HasOne(r => r.Event)
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // One current weather record per event.
            modelBuilder.Entity<Weather>()
                .HasIndex(w => w.EventId)
                .IsUnique();

            modelBuilder.Entity<Weather>()
                .HasOne(w => w.Event)
                .WithMany()
                .HasForeignKey(w => w.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Email addresses must be unique.
            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Store event distances and fees with two decimal places.
            modelBuilder.Entity<Events>()
                .Property(e => e.DistanceKm)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Events>()
                .Property(e => e.EntryFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Routes>()
                .Property(r => r.DistanceKm)
                .HasPrecision(8, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.TemperatureC)
                .HasPrecision(5, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.WindSpeedKph)
                .HasPrecision(6, 2);

            foreach (var property in modelBuilder.Model
    .GetEntityTypes()
    .SelectMany(e => e.GetProperties()))
            {
                if (property.ClrType == typeof(string))
                {
                    property.SetIsUnicode(false);
                }
            }
        }
    }
}



