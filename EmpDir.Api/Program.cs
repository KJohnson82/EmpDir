using EmpDir.Core.Data.Context;
using EmpDir.Core.DTOs;
using EmpDir.Core.Extensions;
using EmpDir.Core.Models;
using EmpDir.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// IDirectoryService implementation using cache
builder.Services.AddScoped<IDirectoryService, DirectoryService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlite(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ===== MINIMAL API ENDPOINTS =====

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("System");

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
            Employees = employees.Select(e => e.ToDto()).ToList(),
            Departments = departments.Select(d => d.ToDto()).ToList(),
            Locations = locations.Select(l => l.ToDto()).ToList(),
            LocationTypes = locationTypes.Select(lt => lt.ToDto()).ToList(),
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
//.RequireAuthorization(); // If you have API key middleware

app.UseHttpsRedirection();


app.Run();

