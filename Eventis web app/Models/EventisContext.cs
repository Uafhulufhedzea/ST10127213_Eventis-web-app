using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Eventis_web_app.Models;

public partial class EventisContext : DbContext
{
    public EventisContext()
    {
    }

    public EventisContext(DbContextOptions<EventisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Venue> Venues { get; set; }

    public virtual DbSet<BookingSummaryView> BookingSummaryViews { get; set; }

protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //new task 2 fix
{
    // Plaintext credentials completely purged for source code protection.
    // The connection string is now safely injected via Program.cs.
}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACDB1045396");

            entity.Property(e => e.BookingId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("Confirmed");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__EventI__778AC167");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__VenueI__787EE5A0");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Events__7944C870751D3F73");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venues__3C57E5D205077F32");
        });

        // Add this new mapping block right here:
        modelBuilder.Entity<BookingSummaryView>(entity =>
        {
            entity.HasNoKey();
            entity.ToView("vw_BookingDetails");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
