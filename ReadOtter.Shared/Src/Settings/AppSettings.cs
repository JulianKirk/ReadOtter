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
}
