using Microsoft.Extensions.Logging;
using ReadOtter.Shared.Data;
using ReadOtter.Shared.Data.Services;

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

            builder.Services.AddDbContext<ReadOtterLibraryDbContext>();
            builder.Services.AddScoped<Seeder>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<EpubContentService>();
			builder.Services.AddScoped<EpubMetadataService>();
            builder.Services.AddScoped<BookCommonService>();
            builder.Services.AddSingleton<InputService>();
#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            return app;
        }
    }
}
