using EmpDir.Core.Data.Context;
using EmpDir.Core.DTOs;
using EmpDir.Core.Extensions;
using EmpDir.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// IDirectoryService implementation using cache
builder.Services.AddScoped<IDirectoryService, DirectoryService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

var app = builder.Build();

// ===== MINIMAL API ENDPOINTS =====
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("System");

app.MapGet("/api/directory/sync", async ([FromServices] IDirectoryService service) =>
{
    try
    {
        // Service now returns DTOs directly - no need to map!
        var employees = await service.GetEmployeesAsync();
        var departments = await service.GetDepartmentsAsync();
        var locations = await service.GetLocationsAsync();
        var locationTypes = await service.GetLoctypesAsync();

        var syncData = new DirectorySyncDto
        {
            Employees = employees,        // Already DTOs!
            Departments = departments,    // Already DTOs!
            Locations = locations,        // Already DTOs!
            LocationTypes = locationTypes, // Already DTOs!
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

app.UseHttpsRedirection();
app.Run();