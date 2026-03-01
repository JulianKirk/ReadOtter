namespace ReadOtter.Shared.Src.Settings;

public enum Theme
{
    Light,
    Dark,
}

public class AppSettings
{
    public Theme Theme { get; set; } = Theme.Dark;

    public bool IsDevMode { get; set; }

    public TimeSpan BookCacheExpiry { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan EpubParsingCacheExpiry { get; set; } = TimeSpan.FromMinutes(60);
}
