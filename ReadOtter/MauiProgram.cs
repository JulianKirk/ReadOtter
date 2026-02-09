using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReadOtter.Shared.Interopt;
using ReadOtter.Shared.Src.Data;
using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Data.Epub;
using ReadOtter.Shared.Src.Services;

namespace ReadOtter
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

            builder.Services.AddMauiBlazorWebView();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "ReadOtterLibrary.db");
            builder.Services.AddDbContext<ReadOtterLibraryDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}")
            );
            builder.Services.AddScoped<SeederService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            builder.Services.AddScoped<IVersOneAdaptor, VersOneAdaptor>();
            builder.Services.AddScoped<IBookProvider, CachedBookProvider>();

            builder.Services.AddScoped<EpubContentService>();
            builder.Services.AddScoped<EpubMetadataService>();
            builder.Services.AddScoped<BookCollectionService>();

            builder.Services.AddScoped<InputService>();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ReadOtterLibraryDbContext>();
                db.Database.Migrate();

                var hasStaleBooks = db
                    .Books.AsEnumerable()
                    .Any(b => b.FilePath != null && !File.Exists(b.FilePath));
                if (!db.Books.Any() || hasStaleBooks)
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<SeederService>();
                    seeder.ClearAllData();
                    seeder.SeedData();
                }
            }

            InputInteropt.SetServiceProvider(app.Services);

            return app;
        }
    }
}
