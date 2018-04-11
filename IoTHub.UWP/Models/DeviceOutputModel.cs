using System;

namespace IoTHub.UWP.Models
{
	public class DeviceOutputModel
	{
		public string DeviceId { get; set; }
		public double Humidity { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Temperature { get; set; }
		public DateTime Timestamp { get; set; }
	}
}