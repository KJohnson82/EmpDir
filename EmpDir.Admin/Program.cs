using EmpDir.Admin.Components;
using EmpDir.Core.Data.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== DATABASE CONFIGURATION =====
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Use PostgreSQL
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
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

// ===== UI SERVICES =====
builder.Services.AddTelerikBlazor();
builder.Services.AddScoped<EmpDir.Admin.Components.Layout.MainLayout>();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// ===== DATABASE CHECK ON STARTUP =====
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Checking database connection...");

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


//using EmpDir.Admin.Components;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddTelerikBlazor();
//builder.Services.AddScoped<EmpDir.Admin.Components.Layout.MainLayout>();


//// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();

//app.MapStaticAssets();
//app.UseAntiforgery();


//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//app.Run();
