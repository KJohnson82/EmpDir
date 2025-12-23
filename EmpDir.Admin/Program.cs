using EmpDir.Admin.Components;
using EmpDir.Core.Data.Context;
using EmpDir.Core.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"CONNECTION STRING: {connectionString}");

// CHANGE THIS: Use DbContextFactory instead of AddDbContext for Blazor Server
builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);

#if DEBUG
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
#endif
});

// ===== UI SERVICES =====
builder.Services.AddTelerikBlazor();
//builder.Services.AddScoped<EmpDir.Admin.Components.Layout.MainLayout>();
builder.Services.AddScoped<EmpDir.Admin.Services.IAuthService, EmpDir.Admin.Services.AuthService>();

builder.Services.AddScoped<ExportData>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ===== DATABASE CHECK ON STARTUP =====
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Checking database connection...");
        await using var context = await contextFactory.CreateDbContextAsync();

        if (await context.Database.CanConnectAsync())
        {
            logger.LogInformation("? Database connection successful");

            // Ensure tables exist
            await context.Database.EnsureCreatedAsync();

            // Log record counts for verification
            var empCount = await context.Employees.CountAsync();
            var deptCount = await context.Departments.CountAsync();
            var locCount = await context.Locations.CountAsync();

            logger.LogInformation("Database contains: {Employees} employees, {Departments} departments, {Locations} locations",
                empCount, deptCount, locCount);
        }
        else
        {
            logger.LogError("? Cannot connect to database. Is PostgreSQL running?");
            logger.LogError("Run: docker-compose up -d");
            throw new Exception("Database connection failed");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed");
        throw;
    }
}

// ===== HTTP PIPELINE =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
