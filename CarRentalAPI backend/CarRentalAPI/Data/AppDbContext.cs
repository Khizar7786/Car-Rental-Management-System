using CarRentalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // all dbsets
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Agreement> Agreements => Set<Agreement>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // user
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();

            e.Property(u => u.Role)
                .HasDefaultValue("Customer");

            e.Property(u => u.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        // vehichle
        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasIndex(v => v.PlateNo).IsUnique();

            e.Property(v => v.PerDayRate)
                .HasColumnType("decimal(18,2)");

            e.Property(v => v.Category)
                .HasConversion<string>();
        });

        // customer
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasIndex(c => c.Email).IsUnique();
            e.HasIndex(c => c.LicenseNo).IsUnique();

            e.HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // agreement
        modelBuilder.Entity<Agreement>(e =>
        {
            e.Property(a => a.RatePerDay).HasColumnType("decimal(18,2)");
            e.Property(a => a.TotalAmount).HasColumnType("decimal(18,2)");

            e.HasOne(a => a.Customer)
                .WithMany(c => c.Agreements)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(a => a.Vehicle)
                .WithMany(v => v.Agreements)
                .HasForeignKey(a => a.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // invoce
        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasIndex(i => i.InvoiceNo).IsUnique();

            e.Property(i => i.Rate).HasColumnType("decimal(18,2)");
            e.Property(i => i.TotalAmount).HasColumnType("decimal(18,2)");

            e.HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(i => i.Vehicle)
                .WithMany(v => v.Invoices)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(i => i.Agreement)
                .WithMany(a => a.Invoices)
                .HasForeignKey(i => i.AgreementId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Admin seeding
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            FullName = "Super Admin",
            Email = "admin@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Phone = "03001234567",
            Role = "Admin",
            IsActive = true,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Dummy vehichles
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle
            {
                Id = 1,
                Make = "Suzuki",
                Model = "Alto",
                Year = 2024,
                PlateNo = "LEA-1234",
                Category = "Hatchback",
                PerDayRate = 4500,
                ImageUrl = "/images/alto.jpg",
                Description = "Fuel-efficient city car",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vehicle
            {
                Id = 2,
                Make = "Suzuki",
                Model = "Cultus",
                Year = 2024,
                PlateNo = "LEA-5678",
                Category = "Hatchback",
                PerDayRate = 5000,
                ImageUrl = "/images/cultus.jpg",
                Description = "Comfortable hatchback",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vehicle
            {
                Id = 3,
                Make = "Suzuki",
                Model = "Wagon R",
                Year = 2024,
                PlateNo = "LEA-9012",
                Category = "Hatchback",
                PerDayRate = 5500,
                ImageUrl = "/images/wagonr.jpg",
                Description = "Spacious family car",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vehicle
            {
                Id = 4,
                Make = "Toyota",
                Model = "Yaris",
                Year = 2024,
                PlateNo = "LEA-3456",
                Category = "Sedan",
                PerDayRate = 6500,
                ImageUrl = "/images/yaris.jpg",
                Description = "Reliable sedan",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vehicle
            {
                Id = 5,
                Make = "Honda",
                Model = "City",
                Year = 2024,
                PlateNo = "LEA-7890",
                Category = "Sedan",
                PerDayRate = 7000,
                ImageUrl = "/images/city.jpg",
                Description = "Premium sedan",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Vehicle
            {
                Id = 6,
                Make = "Toyota",
                Model = "Corolla",
                Year = 2024,
                PlateNo = "LEA-2345",
                Category = "Sedan",
                PerDayRate = 8000,
                ImageUrl = "/images/corolla.jpg",
                Description = "Classic family sedan",
                IsAvailable = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}