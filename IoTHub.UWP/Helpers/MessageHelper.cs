using System;
using System.Text;
using System.Threading.Tasks;
using IoTHub.UWP.Common;
using Microsoft.Azure.Devices;

namespace IoTHub.UWP.Helpers
{
	public static class MessageHelper
	{
		private static async Task<bool> SendMessageToDeviceAsync(string deviceId, string message)
		{
			var successful = false;

			try
			{
				var serviceClient = ServiceClient.CreateFromConnectionString(Constants.DeviceMessagingConnectionString);
				var commandMessage = new Message(Encoding.ASCII.GetBytes(message));
				await serviceClient.SendAsync(deviceId, commandMessage);
				successful = true;
			}
			catch (Exception)
			{
				// ignored
			}

			return successful;
		}

		public static async Task SendMessagesToDevicesAsync(string message)
		{
			var iotHub = new ImportDevicesHelper();
			var amountOfDevicesToTake = 10;
			foreach (var i in await iotHub.GetIoTHubDevicesAsync(amountOfDevicesToTake))
				await SendMessageToDeviceAsync(i.Id, message);
		}
	}
}