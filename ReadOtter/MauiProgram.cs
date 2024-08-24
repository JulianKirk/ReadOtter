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
            builder.Services.AddSingleton<Seeder>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<EpubService>();

#if DEBUG
    		builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
