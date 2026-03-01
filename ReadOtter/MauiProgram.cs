using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReadOtter.Shared.Src.Configuration;
using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Epub;
using ReadOtter.Shared.Src.Services;
using ReadOtter.Shared.Src.Settings;
using Serilog;

namespace ReadOtter;

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

        builder.Services.AddMauiBlazorWebView();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "ReadOtterLibrary.db");
        var connectionString = $"Data Source={dbPath}";

        builder.Services.AddDbContext<ReadOtterLibraryDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Configuration.AddSqliteSettings(connectionString);
        builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

        builder.Services.AddScoped<SeederService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IVersOneAdaptor, VersOneAdaptor>();
        builder.Services.AddScoped<IBookProvider, CachedBookProvider>();

        builder.Services.AddScoped<EpubContentService>();
        builder.Services.AddScoped<EpubMetadataService>();
        builder.Services.AddScoped<BookCollectionService>();

        builder.Services.AddScoped<InputService>();
        builder.Services.AddScoped<LinkService>();
        builder.Services.AddScoped<BookNotificationService>();
        builder.Services.AddScoped<AppSettingsService>();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                Path.Combine(FileSystem.AppDataDirectory, "Logs", "readotter-.log"),
                rollingInterval: RollingInterval.Day)
            .WriteTo.Console()
            .WriteTo.Debug()
            .CreateLogger();

        builder.Logging.AddSerilog(Log.Logger);

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReadOtterLibraryDbContext>();
            db.Database.Migrate();
        }

        return app;
    }
}
