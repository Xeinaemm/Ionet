using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using IoTHub.UWP.Services;

namespace IoTHub.UWP.Activation
{
	internal class DefaultLaunchActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
	{
		private readonly string _navElement;

		public DefaultLaunchActivationHandler(Type navElement) => _navElement = navElement.FullName;

		private NavigationServiceEx NavigationService =>
			Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<NavigationServiceEx>();

		protected override async Task HandleInternalAsync(LaunchActivatedEventArgs args)
		{
			// When the navigation stack isn't restored, navigate to the first page and configure
			// the new page by passing required information in the navigation parameter
			NavigationService.Navigate(_navElement, args.Arguments);

			await Task.CompletedTask;
		}

		protected override bool CanHandleInternal() => NavigationService.Frame.Content == null;
	}
}