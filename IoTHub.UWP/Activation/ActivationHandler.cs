using System.Threading.Tasks;

namespace IoTHub.UWP.Activation
{
	// For more information on application activation see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/activation.md
	internal abstract class ActivationHandler
	{
		public abstract bool CanHandle(object args);

		public abstract Task HandleAsync(object args);
	}

	internal abstract class ActivationHandler<T> : ActivationHandler
		where T : class
	{
		protected abstract Task HandleInternalAsync(T args);

		public override async Task HandleAsync(object args)
		{
			await HandleInternalAsync(args as T);
		}

		public override bool CanHandle(object args) => args is T && CanHandleInternal();

		protected virtual bool CanHandleInternal() => true;
	}
}