using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using IoTHub.UWP.Listeners;
using IoTHub.UWP.Services;
using IoTHub.UWP.ViewModels;

namespace IoTHub.UWP
{
	public sealed partial class App
	{
		private readonly Lazy<ActivationService> _activationService;
		private readonly CharlieActivityListener _charlieActivityListener = new CharlieActivityListener();
		private readonly CharlieWarningListener _charlieWarningListener = new CharlieWarningListener();
		public App()
		{
			InitializeComponent();
			_activationService = new Lazy<ActivationService>(CreateActivationService);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
		}

		private ActivationService ActivationService => _activationService.Value;

		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			_charlieActivityListener.StartListeningAsync();
			_charlieWarningListener.StartListeningAsync();
			if (!args.PrelaunchActivated) await ActivationService.ActivateAsync(args);
		}

		protected override async void OnActivated(IActivatedEventArgs args)
		{
			await ActivationService.ActivateAsync(args);
		}

		private static ActivationService CreateActivationService() => new ActivationService(typeof(PivotViewModel));
	}
}