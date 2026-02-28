using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        builder.Services.AddDbContext<ReadOtterLibraryDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddScoped<SeederService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<IVersOneAdaptor, VersOneAdaptor>();
        builder.Services.AddScoped<IBookProvider, CachedBookProvider>();

        builder.Services.AddScoped<EpubContentService>();
        builder.Services.AddScoped<EpubMetadataService>();
        builder.Services.AddScoped<BookCollectionService>();

        builder.Services.AddScoped<InputService>();
        builder.Services.AddScoped<LinkService>();

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
        builder.Services.Configure<DevSettings>(o => o.IsDevMode = true);
#else
        builder.Services.Configure<DevSettings>(_ => { });
#endif

        var app = builder.Build();

        return app;
    }
}
