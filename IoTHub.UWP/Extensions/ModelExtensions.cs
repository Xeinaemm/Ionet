using IoTHub.UWP.Models;

namespace IoTHub.UWP.Extensions
{
	public static class ModelExtensions
	{
		public static void Hydrate(this FullDeviceDisplayModel deviceInfo, DeviceOutputModel info)
		{
			deviceInfo.DeviceId = info.DeviceId;
			deviceInfo.Temperature = info.Temperature;
			deviceInfo.Humidity = info.Humidity;
			deviceInfo.Timestamp = info.Timestamp;
			deviceInfo.Latitude = info.Latitude;
			deviceInfo.Longitude = info.Longitude;
		}
	}
}