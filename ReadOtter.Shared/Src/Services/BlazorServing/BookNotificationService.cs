namespace ReadOtter.Shared.Src.Services;

public class BookNotificationService
{
    public event Action? OnBooksChanged;

    public void NotifyBooksChanged() => OnBooksChanged?.Invoke();
}
