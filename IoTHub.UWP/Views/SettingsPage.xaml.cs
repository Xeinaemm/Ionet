using IoTHub.UWP.ViewModels;

namespace IoTHub.UWP.Views
{
	public sealed partial class SettingsPage
	{
		//// TODO WTS: Change the URL for your privacy policy in the Resource File, currently set to https://YourPrivacyUrlGoesHere

		public SettingsPage()
		{
			InitializeComponent();
		}

		private SettingsViewModel ViewModel => DataContext as SettingsViewModel;
	}
}