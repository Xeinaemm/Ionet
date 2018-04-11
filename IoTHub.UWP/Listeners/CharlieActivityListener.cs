using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight.Messaging;
using IoTHub.UWP.Common;
using IoTHub.UWP.Helpers;
using IoTHub.UWP.Models;
using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;

namespace IoTHub.UWP.Listeners
{
	public class CharlieActivityListener
	{
		private EventHubClient Client { get; set; }
		private EventHubConsumerGroup ConsumerGroup { get; set; }
		private EventHubReceiver Receiver { get; set; }
		private EventHubReceiver ReceiverSecond { get; set; }

		public async void StartListeningAsync()
		{
			Client = EventHubClient.CreateFromConnectionString(Constants.CharlieActivityEventHubEndpoint,
				Constants.CharlieActivityEventHubName);
			ConsumerGroup = Client.GetDefaultConsumerGroup();
			Receiver = ConsumerGroup.CreateReceiver("0", DateTime.Now.ToUniversalTime());
			ReceiverSecond = ConsumerGroup.CreateReceiver("1", DateTime.Now.ToUniversalTime());
			await Task.Run(StartListeningForCharlieActivityCommandsAsync);
		}

		private async Task StartListeningForCharlieActivityCommandsAsync()
		{
			while (true)
			{
				try
				{
					await GetEventAsync(Receiver.Receive());
					await GetEventAsync(ReceiverSecond.Receive());
				}
				catch
				{
					// ignored
				}
			}
		}

		private static async Task GetEventAsync(EventData eventData)
		{
			if (eventData == null) return;
			var bytes = eventData.GetBytes();
			var payload = Encoding.UTF8.GetString(bytes);
			var charlieInfo = JsonConvert.DeserializeObject<DeviceOutputModel>(payload);
			charlieInfo.Timestamp = charlieInfo.Timestamp.ToUniversalTime();
			Messenger.Default.Send(charlieInfo);
			var storage = ApplicationData.Current.LocalFolder;
			await storage.AppendToFileAsync("devices", charlieInfo);
		}
	}
}