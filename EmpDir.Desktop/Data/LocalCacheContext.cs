using EmpDir.Core.Models;
using Microsoft.EntityFrameworkCore;
using Location = EmpDir.Core.Models.Location;

namespace EmpDir.Desktop.Data;

/// <summary>
/// Local SQLite cache database context for EmpDir.Desktop
/// This is separate from AppDbContext - it's a local read-only cache
/// </summary>
public class LocalCacheContext : DbContext
{
    public LocalCacheContext(DbContextOptions<LocalCacheContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Loctype> Loctypes { get; set; } = null!;
    public DbSet<SyncMetadata> SyncMetadata { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(20);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(60);

            // Relationships (for querying, even though DTOs are flattened)
            entity.HasOne(e => e.EmpLocation)
                .WithMany(l => l.Employees)
                .HasForeignKey(e => e.Location)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.EmpDepartment)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.Department)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.DeptName).IsRequired().HasMaxLength(80);

            entity.HasOne(d => d.DeptLocation)
                .WithMany(l => l.Departments)
                .HasForeignKey(d => d.Location)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Location entity
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(l => l.Id);
            entity.Property(l => l.LocName).IsRequired().HasMaxLength(80);

            entity.HasOne(l => l.LocationType)
                .WithMany(lt => lt.Locations)
                .HasForeignKey(l => l.Loctype)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Loctype entity
        modelBuilder.Entity<Loctype>(entity =>
        {
            entity.ToTable("Loctypes");
            entity.HasKey(lt => lt.Id);
            entity.Property(lt => lt.LoctypeName).IsRequired().HasMaxLength(40);
        });

        // Configure SyncMetadata entity
        modelBuilder.Entity<SyncMetadata>(entity =>
        {
            entity.ToTable("SyncMetadata");
            entity.HasKey(sm => sm.Id);
        });
    }
}

/// <summary>
/// Stores metadata about cache sync operations
/// </summary>
public class SyncMetadata
{
    public int Id { get; set; }
    public DateTime LastSyncTime { get; set; }
    public int EmployeeCount { get; set; }
    public int DepartmentCount { get; set; }
    public int LocationCount { get; set; }
    public int LoctypeCount { get; set; }
    public bool LastSyncSuccessful { get; set; }
    public string? LastSyncMessage { get; set; }
}

