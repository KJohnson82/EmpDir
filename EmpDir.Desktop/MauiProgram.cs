using EmpDir.Core;
using EmpDir.Core.Data;
using EmpDir.Core.Data.Context;
using EmpDir.UI;
using EmpDir.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Telerik.Blazor;

namespace EmpDir.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddTelerikBlazor();

            builder.Services.AddMauiBlazorWebView();

            // Register custom application services.
            builder.Services.AddScoped<EmpDir.Core.Services.SearchService>();
            builder.Services.AddScoped<IDirectoryService, DirectoryService>();
            builder.Services.AddSingleton<LayoutState>();
            
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite($"Filename={FileSystem.AppDataDirectory}/empdir.db");
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
