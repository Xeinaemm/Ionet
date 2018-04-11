using System.Collections.Generic;
using System.Threading.Tasks;
using IoTHub.UWP.Common;
using Microsoft.Azure.Devices;

namespace IoTHub.UWP.Helpers
{
	public class ImportDevicesHelper
	{
		private readonly RegistryManager _registryManager;

		public ImportDevicesHelper()
		{
			_registryManager = RegistryManager.CreateFromConnectionString(Constants.DeviceMessagingConnectionString);
		}

		public async Task<IEnumerable<Device>> GetIoTHubDevicesAsync(int maxCountOfDevices)
		{
			var devices = await _registryManager.GetDevicesAsync(maxCountOfDevices);
			return devices;
		}
	}
}