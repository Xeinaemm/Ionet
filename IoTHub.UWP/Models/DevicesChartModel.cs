using System;

namespace IoTHub.UWP.Models
{
	public class DevicesChartModel
	{
		public string DeviceId { get; set; }
		public DateTime DateTime { get; set; }
		public double Temperature { get; set; }
	}
}