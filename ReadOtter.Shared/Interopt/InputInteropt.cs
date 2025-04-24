using Microsoft.JSInterop;
using ReadOtter.Shared.Data.Services;

namespace ReadOtter.Shared.Interopt
{
	public static class InputInteropt
	{
		static IServiceProvider? serviceProvider;

		public static void SetServiceProvider(IServiceProvider serviceProvider)
		{
            InputInteropt.serviceProvider = serviceProvider;
		}

		[JSInvokable]
		public static void OnKeyDown(string key)
		{
			var inputService = serviceProvider?.GetService(typeof(InputService)) as InputService;
			inputService?.TriggerKeyDown(key);
		}
	}
}
