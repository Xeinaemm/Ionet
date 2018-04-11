using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using IoTHub.UWP.Helpers;
using IoTHub.UWP.Models;

namespace IoTHub.UWP.ViewModels
{
	public class DevicesViewModel : ViewModelBase
	{
		private readonly IDictionary<string, DeviceOutputModel> _devicesDictionary;
		private ObservableCollection<DeviceOutputModel> _devices;
		private DeviceOutputModel _selected;

		public DevicesViewModel()
		{
			Messenger.Default.Register<DeviceOutputModel>(this, ActivityListener);
			_devicesDictionary = new Dictionary<string, DeviceOutputModel>();
			Devices = new ObservableCollection<DeviceOutputModel>();
		}

		public ObservableCollection<DeviceOutputModel> Devices
		{
			get => _devices;
			set => Set(ref _devices, value);
		}

		public DeviceOutputModel Selected
		{
			get => _selected;
			set => Set(ref _selected, value);
		}

		private async void ActivityListener(DeviceOutputModel obj)
		{
			await SendDataToChartVmAsync();
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			if (_devicesDictionary.ContainsKey(obj.DeviceId))
			{
				await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					var deviceOutputModel = Devices.First(x => x.DeviceId == obj.DeviceId);
					var index = Devices.IndexOf(deviceOutputModel);
					Devices[index] = obj;
					if (Selected == null)
						Selected = obj;
					else if (Selected.DeviceId == obj.DeviceId)
						Selected = obj;
				});
			}
			else
			{
				_devicesDictionary[obj.DeviceId] = obj;
				await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					Devices.Add(_devicesDictionary[obj.DeviceId]);
					if (Selected == null)
						Selected = obj;
				});
			}
		}

		private static async Task<IList<DeviceOutputModel>> GetDevicesFromFileAsync()
		{
			var storageFolder = ApplicationData.Current.LocalFolder;
			var devices = await storageFolder.ReadAsync<IList<DeviceOutputModel>>("devices");
			return devices;
		}

		private async Task SendDataToChartVmAsync()
		{
			var devices = await GetDevicesFromFileAsync();
			if (devices == null) return;
			if (Selected == null) return;
			var query = devices.Where(x => x.DeviceId == Selected.DeviceId).ToList();
			Messenger.Default.Send(query);
		}
	}
}