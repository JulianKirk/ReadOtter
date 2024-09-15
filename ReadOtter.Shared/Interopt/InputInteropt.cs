using Microsoft.JSInterop;
using ReadOtter.Shared.Data.Services;

namespace ReadOtter.Shared.Interopt
{
	public static class InputInteropt
	{
		private static IServiceProvider _serviceProvider;

		public static void SetServiceProvider(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		[JSInvokable]
		public static void OnKeyDown(string key)
		{
			var inputService = _serviceProvider.GetService(typeof(InputService)) as InputService;
			inputService?.TriggerKeyDown(key);
		}
	}
}
