using Windows.UI.Xaml;
using IoTHub.UWP.Common;

namespace IoTHub.UWP.Controls
{
	public sealed partial class CharlieDeviceControl
	{
		public static readonly DependencyProperty DeviceIdProperty =
			DependencyProperty.Register("DeviceId", typeof(string), typeof(CharlieDeviceControl), null);

		public static readonly DependencyProperty ZoomDeepLevelProperty =
			DependencyProperty.Register("ZoomDeepLevel", typeof(Visibility), typeof(CharlieDeviceControl), null);

		public static readonly DependencyProperty StatusProperty =
			DependencyProperty.Register("Status", typeof(DeviceStatus), typeof(CharlieDeviceControl), null);


		public CharlieDeviceControl()
		{
			InitializeComponent();
		}

		public string DeviceId
		{
			get => GetValue(DeviceIdProperty) as string;
			set => SetValue(DeviceIdProperty, value);
		}

		public Visibility ZoomDeepLevel
		{
			get => (Visibility) GetValue(ZoomDeepLevelProperty);
			set => SetValue(ZoomDeepLevelProperty, value);
		}

		public DeviceStatus Status
		{
			get => (DeviceStatus)GetValue(StatusProperty);
			set => SetValue(StatusProperty, value);
		}
	}
}