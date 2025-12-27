using Microsoft.EntityFrameworkCore;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Enums;

namespace EmptySlot.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ServiceCategory> ServiceCategories { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<ProviderLocation> ProviderLocations { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<StaffMember> StaffMembers { get; set; }
    public DbSet<StaffSchedule> StaffSchedules { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Profile configuration
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.UserType)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasOne(e => e.Provider)
                .WithOne(e => e.User)
                .HasForeignKey<Provider>(e => e.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ServiceCategory configuration
        modelBuilder.Entity<ServiceCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Slug).IsUnique();

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Provider configuration
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.RatingAverage).HasPrecision(3, 2);

            // Indexes for performance
            entity.HasIndex(e => e.Verified);
            entity.HasIndex(e => e.IsFeatured);
            entity.HasIndex(e => e.RatingAverage);
            entity.HasIndex(e => new { e.Verified, e.IsFeatured });

            entity.HasMany(e => e.Locations)
                .WithOne(e => e.Provider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Services)
                .WithOne(e => e.Provider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.StaffMembers)
                .WithOne(e => e.Provider)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ProviderLocation configuration
        modelBuilder.Entity<ProviderLocation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.AddressLine1).IsRequired().HasMaxLength(255);
            entity.Property(e => e.City).IsRequired().HasMaxLength(100);
            entity.Property(e => e.StateProvince).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PostalCode).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Country).IsRequired().HasMaxLength(100);

            // Indexes for location-based searches
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => new { e.City, e.StateProvince });
        });

        // Service configuration
        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.DepositRequired).HasPrecision(10, 2);

            entity.HasOne(e => e.Category)
                .WithMany(e => e.Services)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StaffMember configuration
        modelBuilder.Entity<StaffMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);

            entity.HasMany(e => e.Schedules)
                .WithOne(e => e.Staff)
                .HasForeignKey(e => e.StaffMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.TimeSlots)
                .WithOne(e => e.Staff)
                .HasForeignKey(e => e.StaffMemberId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // StaffSchedule configuration
        modelBuilder.Entity<StaffSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DayOfWeek)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Index for efficient schedule queries
            entity.HasIndex(e => new { e.StaffMemberId, e.DayOfWeek, e.IsActive });
        });

        // TimeSlot configuration
        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Indexes for efficient time slot queries
            entity.HasIndex(e => new { e.StaffMemberId, e.Date, e.IsAvailable });
            entity.HasIndex(e => new { e.Date, e.IsAvailable });
            entity.HasIndex(e => e.AppointmentId);

            entity.HasOne(e => e.Appointment)
                .WithOne()
                .HasForeignKey<TimeSlot>(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Appointment configuration
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BookingReference).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.BookingReference).IsUnique();
            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.PaymentStatus)
                .HasConversion<string>()
                .HasMaxLength(20);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.DepositPaid).HasPrecision(10, 2);

            // Indexes for appointment queries
            entity.HasIndex(e => e.StartDateTime);
            entity.HasIndex(e => new { e.ProviderId, e.StartDateTime });
            entity.HasIndex(e => new { e.CustomerId, e.StartDateTime });
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.Customer)
                .WithMany(e => e.CustomerAppointments)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Provider)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Location)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Service)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Staff)
                .WithMany(e => e.Appointments)
                .HasForeignKey(e => e.StaffId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();

            entity.HasOne(e => e.Customer)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Provider)
                .WithMany(e => e.Reviews)
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Favorite configuration
        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CustomerId, e.ProviderId }).IsUnique();

            entity.HasOne(e => e.Customer)
                .WithMany(e => e.Favorites)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
