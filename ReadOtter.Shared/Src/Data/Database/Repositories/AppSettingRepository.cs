using ReadOtter.Shared.Src.Data.Models;

namespace ReadOtter.Shared.Src.Data.Database.Repositories;

public class AppSettingRepository : Repository<AppSetting>, IAppSettingRepository
{
    public AppSettingRepository(ReadOtterLibraryDbContext context)
        : base(context)
    {
    }

    public AppSetting? GetSettingByName(string name)
    {
        return DbContext.AppSettings.FirstOrDefault(s => s.SettingName == name);
    }

    public IEnumerable<AppSetting> GetAllSettings()
    {
        return DbContext.AppSettings.ToList();
    }

    public void AddOrUpdateSetting(string name, string value)
    {
        var existing = GetSettingByName(name);

        if (existing != null)
        {
            existing.SettingValue = value;
            Update(existing);
        }
        else
        {
            Add(new AppSetting
            {
                Id = Guid.NewGuid(),
                SettingName = name,
                SettingValue = value,
            });
        }
    }
}
