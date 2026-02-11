using Microsoft.JSInterop;

namespace ReadOtter.Shared.Src.Services;

public class InputService : IAsyncDisposable
{
    private readonly IJSRuntime jsRuntime;
    private DotNetObjectReference<InputService>? dotNetRef;

    public InputService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public event Action<string>? OnKeyDown;

    public async Task Initialize()
    {
        dotNetRef = DotNetObjectReference.Create(this);
        await jsRuntime.InvokeVoidAsync("InputHandler.register", dotNetRef);
    }

    [JSInvokable]
    public void HandleKeyDown(string key)
    {
        OnKeyDown?.Invoke(key);
    }

    public async ValueTask DisposeAsync()
    {
        if (dotNetRef != null)
        {
            await jsRuntime.InvokeVoidAsync("InputHandler.unregister");
            dotNetRef.Dispose();
            dotNetRef = null;
        }
    }
}
