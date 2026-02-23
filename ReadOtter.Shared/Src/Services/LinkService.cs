using Microsoft.JSInterop;

namespace ReadOtter.Shared.Src.Services;

public class LinkService : IAsyncDisposable
{
    private readonly IJSRuntime jsRuntime;
    private DotNetObjectReference<LinkService>? dotNetRef;

    public LinkService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public event Action<string>? OnLinkClick;

    public async Task Initialize(string containerSelector)
    {
        dotNetRef = DotNetObjectReference.Create(this);
        await jsRuntime.InvokeVoidAsync("LinkHandler.register", dotNetRef, containerSelector);
    }

    [JSInvokable]
    public void HandleLinkClick(string href)
    {
        OnLinkClick?.Invoke(href);
    }

    public async ValueTask DisposeAsync()
    {
        if (dotNetRef != null)
        {
            await jsRuntime.InvokeVoidAsync("LinkHandler.unregister", ".ReadView-Content");
            dotNetRef.Dispose();
            dotNetRef = null;
        }
    }
}
