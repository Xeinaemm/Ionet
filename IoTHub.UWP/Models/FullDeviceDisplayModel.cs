using System;
using GalaSoft.MvvmLight;

namespace IoTHub.UWP.Models
{
	public class FullDeviceDisplayModel : ViewModelBase
	{
		private string _deviceId;
		private double _humidity;
		private double _latitude;
		private double _longitude;
		private double _temperature;
		private DateTime _timestamp;

		public string DeviceId
		{
			get => _deviceId;
			set => Set(ref _deviceId, value);
		}

		public double Humidity
		{
			get => _humidity;
			set => Set(ref _humidity, value);
		}

		public double Latitude
		{
			get => _latitude;
			set => Set(ref _latitude, value);
		}

		public double Longitude
		{
			get => _longitude;
			set => Set(ref _longitude, value);
		}

		public double Temperature
		{
			get => _temperature;
			set => Set(ref _temperature, value);
		}

		public DateTime Timestamp
		{
			get => _timestamp;
			set => Set(ref _timestamp, value);
		}
	}
}