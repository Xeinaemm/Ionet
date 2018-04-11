using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using IoTHub.UWP.Common;
using IoTHub.UWP.Controls;
using IoTHub.UWP.Helpers;
using IoTHub.UWP.Models;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace IoTHub.UWP.ViewModels
{
	public class MapViewModel : ViewModelBase
	{
		private readonly IDictionary<string, DeviceDisplayModel> _devicesDictionary;
		private ObservableCollection<DeviceDisplayModel> _devices;
		private MapControl _map;
		private double _zoomLevel;

		public MapViewModel()
		{
			Messenger.Default.Register<DeviceOutputModel>(this, ActivityListener);
			Messenger.Default.Register<AlertModel>(this, WarningListener);
			_devicesDictionary = new Dictionary<string, DeviceDisplayModel>();
			Devices = new ObservableCollection<DeviceDisplayModel>();
			ZoomLevel = Constants.InitialZoomLevel;
		}

		public static string MapServiceToken => Constants.MapServiceToken;

		public double ZoomLevel
		{
			get => _zoomLevel;
			set => Set(ref _zoomLevel, value);
		}

		public ObservableCollection<DeviceDisplayModel> Devices
		{
			get => _devices;
			set => Set(ref _devices, value);
		}

		private async void WarningListener(AlertModel obj)
		{
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				var temp = Devices.FirstOrDefault(x => x.DisplayName == obj.DeviceId);
				if (temp == null) return;
				await CheckWarningsAsync(temp, obj);
			});
		}

		private async void ActivityListener(DeviceOutputModel obj)
		{
			if (_map == null) return;
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			if (_devicesDictionary.ContainsKey(obj.DeviceId))
			{
				await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
				{
					var deviceOutputModel = Devices.First(x => x.DisplayName == obj.DeviceId);
					var index = Devices.IndexOf(deviceOutputModel);
					Devices[index] = new DeviceDisplayModel
					{
						DisplayName = obj.DeviceId,
						Location = new Geopoint(new BasicGeoposition
						{
							Latitude = obj.Latitude,
							Longitude = obj.Longitude
						})
					};
					await UpdateFlightInformationAsync(obj);
				});
			}
			else
			{
				_devicesDictionary[obj.DeviceId] = new DeviceDisplayModel
				{
					DisplayName = obj.DeviceId,
					Location = new Geopoint(new BasicGeoposition
					{
						Latitude = obj.Latitude,
						Longitude = obj.Longitude
					})
				};
				await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { Devices.Add(_devicesDictionary[obj.DeviceId]); });
			}
		}

		private async Task UpdateFlightInformationAsync(DeviceOutputModel info)
		{
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			var deviceDisplayModel = Devices.FirstOrDefault(w => w.DisplayName.Equals(info.DeviceId));
			await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				if (deviceDisplayModel == null)
				{
					Devices.Add(new DeviceDisplayModel
					{
						DisplayName = info.DeviceId,
						ZoomDeepLevel = Visibility.Collapsed,
						Location = Constants.AreaCenter,

						FullDeviceDisplayModel = new FullDeviceDisplayModel
						{
							DeviceId = info.DeviceId,
							Humidity = info.Humidity,
							Temperature = info.Temperature,
							Timestamp = info.Timestamp,
							Longitude = info.Longitude,
							Latitude = info.Latitude
						},

						Status = DeviceStatus.Ok
					});
				}
				else
				{
					deviceDisplayModel.DisplayName = info.DeviceId;
					deviceDisplayModel.FullDeviceDisplayModel = new FullDeviceDisplayModel
					{
						Humidity = info.Humidity,
						Temperature = info.Temperature,
						Timestamp = info.Timestamp,
						Longitude = info.Longitude,
						Latitude = info.Latitude
					};

					deviceDisplayModel.Status = DeviceStatus.Ok;

					var charlieDeviceControl = _map.FindDescendants<CharlieDeviceControl>()
						.FirstOrDefault(w =>
							w.DeviceId != null && w.DeviceId.Equals(deviceDisplayModel.DisplayName, StringComparison.OrdinalIgnoreCase));

					if (charlieDeviceControl != null)
						MapControl.SetLocation(charlieDeviceControl, new Geopoint(new BasicGeoposition
						{
							Latitude = info.Latitude,
							Longitude = info.Longitude
						}));
				}

				await Task.Delay(10);
			});
		}

		private static async Task CheckWarningsAsync(DeviceDisplayModel obj, AlertModel alertModel)
		{
			var oldStatus = obj.Status;
			obj.Status = DeviceStatus.Warning;

			if (oldStatus == DeviceStatus.Ok && obj.Status == DeviceStatus.Warning)
				await SendWarningMessageAsync(alertModel.Alert);
		}

		private static async Task SendWarningMessageAsync(string message)
		{
			await MessageHelper.SendMessagesToDevicesAsync(message);
		}

		public void Initialize(MapControl map)
		{
			_map = map;
		}
	}
}