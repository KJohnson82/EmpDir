#nullable disable

using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Models;

namespace EmpDir.Core.Data.Context
{
    /// Represents the database session for the application.
    /// This class is the main entry point for querying and saving data using Entity Framework Core.
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        #region DbSet Properties

        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Loctype> Loctypes { get; set; }

        #endregion

        /// Configures the database model using the Fluent API.
        /// This configuration is database-agnostic and works with both SQLite and PostgreSQL.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== DEPARTMENT ENTITY CONFIGURATION =====
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Active).HasDefaultValue(true);

                // PostgreSQL: CURRENT_TIMESTAMP, SQLite: CURRENT_TIMESTAMP
                entity.Property(e => e.RecordAdd)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.DeptName)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.DeptEmail).HasMaxLength(60);
                entity.Property(e => e.DeptPhone).HasMaxLength(15);
                entity.Property(e => e.DeptFax).HasMaxLength(15);
                entity.Property(e => e.DeptManager).HasMaxLength(60);

                // Relationship: Department -> Location
                entity.HasOne(d => d.DeptLocation)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== EMPLOYEE ENTITY CONFIGURATION =====
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employees");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Active).HasDefaultValue(true);

                entity.Property(e => e.RecordAdd)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.JobTitle)
                    .IsRequired()
                    .HasMaxLength(60);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.CellNumber).HasMaxLength(15);
                entity.Property(e => e.Extension).HasMaxLength(10);
                entity.Property(e => e.NetworkId).HasMaxLength(30);
                entity.Property(e => e.EmpAvatar).HasMaxLength(255);

                // Relationship: Employee -> Department
                entity.HasOne(d => d.EmpDepartment)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship: Employee -> Location
                entity.HasOne(d => d.EmpLocation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Location)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== LOCATION ENTITY CONFIGURATION =====
            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Locations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Active).HasDefaultValue(true);

                entity.Property(e => e.RecordAdd)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LocName)
                    .IsRequired()
                    .HasMaxLength(80);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Zipcode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.PhoneNumber).HasMaxLength(15);
                entity.Property(e => e.FaxNumber).HasMaxLength(15);
                entity.Property(e => e.Email).HasMaxLength(60);
                entity.Property(e => e.Hours).HasMaxLength(50);
                entity.Property(e => e.AreaManager).HasMaxLength(60);
                entity.Property(e => e.StoreManager).HasMaxLength(60);

                // Relationship: Location -> Loctype
                entity.HasOne(d => d.LocationType)
                    .WithMany(p => p.Locations)
                    .HasForeignKey(d => d.Loctype)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== LOCTYPE ENTITY CONFIGURATION =====
            modelBuilder.Entity<Loctype>(entity =>
            {
                entity.ToTable("Loctypes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.LoctypeName)
                    .IsRequired()
                    .HasMaxLength(40);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
