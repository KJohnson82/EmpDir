
using EmpDir.Core.Data.Context;
using EmpDir.Core.DTOs;
using EmpDir.Core.Extensions;
using EmpDir.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== DATABASE CONFIGURATION =====
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Use PostgreSQL instead of SQLite
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Enable retry on failure for transient errors (network blips, etc.)
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });

#if DEBUG
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
#endif
});

// ===== SERVICE REGISTRATION =====
builder.Services.AddScoped<IDirectoryService, DirectoryService>();

var app = builder.Build();

// ===== DATABASE INITIALIZATION =====
// Ensure database schema exists on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Checking database connection...");

        // Test connection first
        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("Database connection successful!");

            // Create tables if they don't exist
            // For production, consider using migrations instead
            await context.Database.EnsureCreatedAsync();

            logger.LogInformation("Database schema ready!");
        }
        else
        {
            logger.LogError("Cannot connect to database. Is PostgreSQL running?");
            throw new Exception("Database connection failed");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        logger.LogError("Make sure PostgreSQL is running: docker-compose up -d");
        throw;
    }
}

// ===== MINIMAL API ENDPOINTS =====

// Health check endpoint
app.MapGet("/health", async ([FromServices] AppDbContext context) =>
{
    try
    {
        // Actually check database connectivity
        var canConnect = await context.Database.CanConnectAsync();

        return Results.Ok(new
        {
            status = canConnect ? "healthy" : "degraded",
            database = canConnect ? "connected" : "disconnected",
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new
        {
            status = "unhealthy",
            database = "error",
            error = ex.Message,
            timestamp = DateTime.UtcNow
        });
    }
})
.WithName("HealthCheck")
.WithTags("System");

// Directory sync endpoint
app.MapGet("/api/directory/sync", async ([FromServices] IDirectoryService service) =>
{
    try
    {
        var employees = await service.GetEmployeesAsync();
        var departments = await service.GetDepartmentsAsync();
        var locations = await service.GetLocationsAsync();
        var locationTypes = await service.GetLoctypesAsync();

        var syncData = new DirectorySyncDto
        {
            Employees = employees,
            Departments = departments,
            Locations = locations,
            LocationTypes = locationTypes,
            Timestamp = DateTime.UtcNow
        };

        return Results.Ok(syncData);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error retrieving directory data: {ex.Message}");
    }
})
.WithName("DirectorySync")
.WithTags("Directory");

// Database info endpoint (useful for debugging)
app.MapGet("/api/system/dbinfo", async ([FromServices] AppDbContext context) =>
{
    try
    {
        var employeeCount = await context.Employees.CountAsync();
        var departmentCount = await context.Departments.CountAsync();
        var locationCount = await context.Locations.CountAsync();
        var loctypeCount = await context.Loctypes.CountAsync();

        return Results.Ok(new
        {
            provider = "PostgreSQL",
            employees = employeeCount,
            departments = departmentCount,
            locations = locationCount,
            locationTypes = loctypeCount,
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error getting database info: {ex.Message}");
    }
})
.WithName("DatabaseInfo")
.WithTags("System");

app.UseHttpsRedirection();
app.Run();


