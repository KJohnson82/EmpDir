#nullable disable

using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;
using EmpDir.Core.Models;

namespace EmpDir.Core.Data.Context
{

    /// <summary>
    /// Represents the database session for the application.
    /// This class is the main entry point for querying and saving data using Entity Framework Core.
    /// 
    /// UPDATED: Now compatible with both SQLite and PostgreSQL.
    /// </summary>
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

        /// <summary>
        /// Configures the database model using the Fluent API.
        /// This configuration is database-agnostic and works with both SQLite and PostgreSQL.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ===== DEPARTMENT ENTITY CONFIGURATION =====
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Departments");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Active).HasDefaultValue(true);

                // Use HasDefaultValueSql for database-agnostic timestamp
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

//    /// Represents the database session for the application.
//    /// This class is the main entry point for querying and saving data using Entity Framework Core.
//    /// It defines the entity sets (DbSet) that correspond to tables in the database.

//    public partial class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
//    {

//        /// Initializes a new instance of the AppDbContext class.
//        /// This constructor allows for the database connection and other options to be configured
//        /// and passed in via dependency injection.

//        /// <param name="options">The options for this context.</param>
//        public AppDbContext(DbContextOptions<AppDbContext> options)
//        : base(options) { }

//        #region DbSet Properties


//        /// Represents the 'Departments' table in the database.
//        /// Used to query and manage Department entities.

//        public virtual DbSet<Department> Departments { get; set; }


//        /// Represents the 'Employees' table in the database.
//        /// Used to query and manage Employee entities.

//        public virtual DbSet<Employee> Employees { get; set; }


//        /// Represents the 'Locations' table in the database.
//        /// Used to query and manage Location entities.

//        public virtual DbSet<Location> Locations { get; set; }


//        /// Represents the 'Loctypes' (Location Types) table in the database.
//        /// Used to query and manage location type classifications.

//        public virtual DbSet<Loctype> Loctypes { get; set; }

//        #endregion


//        /// Configures the database model using the Fluent API.
//        /// This method is called by Entity Framework Core when the model for the context is being created.
//        /// It is used to define table relationships, constraints, default values, and other data annotations.

//        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // ===== DEPARTMENT ENTITY CONFIGURATION =====
//            modelBuilder.Entity<Department>(entity =>
//            {
//                // Set the default value for the 'Active' property to true for new departments.
//                entity.Property(e => e.Active).HasDefaultValue(true);

//                // Configure the 'RecordAdd' column to automatically set the timestamp on creation.
//                entity.Property(e => e.RecordAdd).HasColumnType("DATETIME DEFAULT CURRENT_TIMESTAMP");

//                // Define the relationship: a Department belongs to one Location.
//                // The foreign key is the 'Location' property in the Department entity.
//                entity.HasOne(d => d.DeptLocation).WithMany(p => p.Departments).HasForeignKey(d => d.Location);
//            });

//            // ===== EMPLOYEE ENTITY CONFIGURATION =====
//            modelBuilder.Entity<Employee>(entity =>
//            {
//                // Set the default value for the 'Active' property to true for new employees.
//                entity.Property(e => e.Active).HasDefaultValue(true);

//                // Configure the 'RecordAdd' column to automatically set the timestamp on creation.
//                entity.Property(e => e.RecordAdd).HasColumnType("DATETIME DEFAULT CURRENT_TIMESTAMP");

//                // Define the relationship: an Employee belongs to one Department.
//                // The foreign key is the 'Department' property in the Employee entity.
//                entity.HasOne(d => d.EmpDepartment).WithMany(p => p.Employees).HasForeignKey(d => d.Department);

//                // Define the relationship: an Employee is assigned to one Location.
//                // The foreign key is the 'Location' property in the Employee entity.
//                entity.HasOne(d => d.EmpLocation).WithMany(p => p.Employees).HasForeignKey(d => d.Location);
//            });

//            // ===== LOCATION ENTITY CONFIGURATION =====
//            modelBuilder.Entity<Location>(entity =>
//            {
//                // Set the default value for the 'Active' property to true for new locations.
//                entity.Property(e => e.Active).HasDefaultValue(true);

//                // Configure the 'RecordAdd' column to automatically set the timestamp on creation.
//                entity.Property(e => e.RecordAdd).HasColumnType("DATETIME DEFAULT CURRENT_TIMESTAMP");

//                // Define the relationship: a Location has one LocationType.
//                // The foreign key is the 'Loctype' property in the Location entity.
//                entity.HasOne(d => d.LocationType).WithMany(p => p.Locations).HasForeignKey(d => d.Loctype);
//            });

//            // Call the partial method to allow for further model configuration in another file.
//            OnModelCreatingPartial(modelBuilder);
//        }


//        /// Provides an extensibility point for model configuration.
//        /// This partial method can be implemented in another partial class file to add
//        /// custom model configurations without modifying this auto-generated file.

//        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
//        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//    }
//}

/*
=================================================
NOTES FOR USING EF CORE POWER TOOLS
=================================================

If you need to re-scaffold the database context and models later:

1. Point EF Core Power Tools to your EmpDir.Core project.
2. Choose the DbContext output folder as 'Context'.
3. Choose the Model output folder as 'Models'.

This will ensure the generated files are placed in the correct locations within the project structure.
*/
