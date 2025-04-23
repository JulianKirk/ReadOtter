namespace ReadOtter.Shared.Data.Services.BlazorConsumed
{
	public class InputService
	{
		public event Action<string> OnKeyDown;

		public void TriggerKeyDown(string key)
		{
			OnKeyDown?.Invoke(key);
		}
	}
}
