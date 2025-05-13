using Microsoft.EntityFrameworkCore;
using EventEaseProject.Models;

namespace EventEaseProject.Data
{
    public class EEDbContext : DbContext
    {
        public EEDbContext(DbContextOptions<EEDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingView> BookingViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.EventDate).HasColumnType("datetime").IsRequired(); // Updated to datetime
                entity.Property(e => e.EventDescription).HasMaxLength(500).IsRequired();
                entity.HasMany(e => e.Bookings)
                      .WithOne(b => b.Event)
                      .HasForeignKey(b => b.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(v => v.VenueId);
                entity.Property(v => v.VenueName).HasMaxLength(250).IsRequired();
                entity.Property(v => v.VenueLocation).HasMaxLength(250).IsRequired();
                entity.Property(v => v.VenueCapacity).IsRequired();
                entity.Property(v => v.VenueImage).IsRequired();
                entity.HasIndex(v => new { v.VenueName, v.VenueLocation }).IsUnique();
                entity.HasMany(v => v.Bookings)
                      .WithOne(b => b.Venue)
                      .HasForeignKey(b => b.VenueId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.BookingId);
                entity.Property(b => b.EventId).IsRequired();
                entity.Property(b => b.VenueId).IsRequired();
                entity.Property(b => b.BookingDate).HasColumnType("datetime").IsRequired();
                entity.HasIndex(b => new { b.EventId, b.VenueId, b.BookingDate }).IsUnique();
            });

            modelBuilder.Entity<BookingView>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("BookingView");
            });
        }
    }
}