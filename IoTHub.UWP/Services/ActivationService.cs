using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTHub.UWP.Activation;
using IoTHub.UWP.ViewModels;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace IoTHub.UWP.Services
{
	internal class ActivationService
	{
		private readonly Type _defaultNavItem;
		private readonly Lazy<UIElement> _shell;

		public ActivationService(Type defaultNavItem, Lazy<UIElement> shell = null)
		{
			_shell = shell;
			_defaultNavItem = defaultNavItem;
		}

		private static ViewModelLocator Locator => Application.Current.Resources["Locator"] as ViewModelLocator;

		private static NavigationServiceEx NavigationService => Locator.NavigationService;

		public async Task ActivateAsync(object activationArgs)
		{
			if (IsInteractive(activationArgs))
			{
				// Initialize things like registering background task before the app is loaded
				await InitializeAsync();

				// Do not repeat app initialization when the Window already has content,
				// just ensure that the window is active
				if (Window.Current.Content == null)
				{
					// Create a Frame to act as the navigation context and navigate to the first page
					Window.Current.Content = _shell?.Value ?? new Frame();
					NavigationService.NavigationFailed += (sender, e) => throw e.Exception;
					NavigationService.Navigated += Frame_Navigated;
					if (SystemNavigationManager.GetForCurrentView() != null)
						SystemNavigationManager.GetForCurrentView().BackRequested += ActivationService_BackRequested;
				}
			}

			var activationHandler = GetActivationHandlers()
				.FirstOrDefault(h => h.CanHandle(activationArgs));

			if (activationHandler != null) await activationHandler.HandleAsync(activationArgs);

			if (IsInteractive(activationArgs))
			{
				var defaultHandler = new DefaultLaunchActivationHandler(_defaultNavItem);
				if (defaultHandler.CanHandle(activationArgs)) await defaultHandler.HandleAsync(activationArgs);

				Window.Current.Activate();

				await StartupAsync();
			}
		}

		private static async Task InitializeAsync()
		{
			await ThemeSelectorService.InitializeAsync();
			await Task.CompletedTask;
		}

		private static async Task StartupAsync()
		{
			ThemeSelectorService.SetRequestedTheme();
			await Task.CompletedTask;
		}

		private static IEnumerable<ActivationHandler> GetActivationHandlers()
		{
			yield break;
		}

		private static bool IsInteractive(object args) => args is IActivatedEventArgs;

		private static void Frame_Navigated(object sender, NavigationEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = NavigationService.CanGoBack
				? AppViewBackButtonVisibility.Visible
				: AppViewBackButtonVisibility.Collapsed;
		}

		private static void ActivationService_BackRequested(object sender, BackRequestedEventArgs e)
		{
			if (!NavigationService.CanGoBack) return;
			NavigationService.GoBack();
			e.Handled = true;
		}
	}
}
