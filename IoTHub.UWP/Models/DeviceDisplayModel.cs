using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using IoTHub.UWP.Common;

namespace IoTHub.UWP.Models
{
	public class DeviceDisplayModel : ViewModelBase
	{
		private string _displayName;
		private FullDeviceDisplayModel _fullDeviceDisplayModel;
		private Geopoint _location;
		private DeviceStatus _status;
		private Visibility _zoomDeepLevel;

		public string DisplayName
		{
			get => _displayName;
			set => Set(ref _displayName, value);
		}

		public Geopoint Location
		{
			get => _location;
			set => Set(ref _location, value);
		}

		public FullDeviceDisplayModel FullDeviceDisplayModel
		{
			get => _fullDeviceDisplayModel;
			set => Set(ref _fullDeviceDisplayModel, value);
		}

		public DeviceStatus Status
		{
			get => _status;
			set => Set(ref _status, value);
		}

		public Visibility ZoomDeepLevel
		{
			get => _zoomDeepLevel;
			set => Set(ref _zoomDeepLevel, value);
		}
	}
}