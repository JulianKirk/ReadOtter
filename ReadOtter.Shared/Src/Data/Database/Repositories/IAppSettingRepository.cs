using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database.Repositories;

public interface IAppSettingRepository
{
    AppSetting? GetSettingByName(string name);

    IEnumerable<AppSetting> GetAllSettings();

    void AddOrUpdateSetting(string name, string value);
}
