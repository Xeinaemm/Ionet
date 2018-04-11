using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using IoTHub.UWP.Models;

namespace IoTHub.UWP.ViewModels
{
	public class DevicesChartViewModel : ViewModelBase
	{
		private ObservableCollection<DevicesChartModel> _deviceChart;

		public DevicesChartViewModel()
		{
			DeviceChart = new ObservableCollection<DevicesChartModel>();
			Messenger.Default.Register<List<DeviceOutputModel>>(this, ActivityListener);
		}

		public ObservableCollection<DevicesChartModel> DeviceChart
		{
			get => _deviceChart;
			set => Set(ref _deviceChart, value);
		}

		private async void ActivityListener(List<DeviceOutputModel> obj)
		{
			if (!obj.Any()) return;
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
			await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				DeviceChart.Clear();
				obj.Sort((x,y) => DateTime.Compare(x.Timestamp, y.Timestamp));
				foreach (var deviceOutputModel in obj)
					DeviceChart.Add(new DevicesChartModel
					{
						DeviceId = deviceOutputModel.DeviceId,
						DateTime = deviceOutputModel.Timestamp,
						Temperature = deviceOutputModel.Temperature
					});
			});
		}
	}
}