using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReadOtter.Shared.Src.Configuration;
using ReadOtter.Shared.Src.Data.Database;
using ReadOtter.Shared.Src.Settings;

namespace ReadOtter.Shared.Src.Services;

public class AppSettingsService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IOptionsMonitor<AppSettings> optionsMonitor;
    private readonly IConfiguration configuration;

    public AppSettingsService(
        IUnitOfWork unitOfWork,
        IOptionsMonitor<AppSettings> optionsMonitor,
        IConfiguration configuration)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.optionsMonitor = optionsMonitor;
        this.configuration = configuration;
    }

    public AppSettings Settings => optionsMonitor.CurrentValue;

    public void SaveSetting(string name, string value)
    {
        unitOfWork.AppSettingRepository.AddOrUpdateSetting(name, value);
        unitOfWork.Commit();

        if (configuration is IConfigurationRoot root)
        {
            var provider = root.Providers
                .OfType<DatabaseConfigurationProvider>()
                .FirstOrDefault();
            provider?.Reload();
        }
    }
}
