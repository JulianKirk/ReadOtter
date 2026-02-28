namespace ReadOtter.Shared.Src.Settings;

public enum Theme
{
    Light,
    Dark,
}

public class AppSettings
{
    public Theme Theme { get; set; } = Theme.Light;

    public bool IsDevMode { get; set; }
}
