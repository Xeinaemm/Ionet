using System;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using IoTHub.UWP.Common;
using IoTHub.UWP.Models;
using Newtonsoft.Json;
using ppatierno.AzureSBLite.Messaging;

namespace IoTHub.UWP.Listeners
{
	public class CharlieWarningListener
	{
		private EventHubClient Client { get; set; }
		private EventHubConsumerGroup ConsumerGroup { get; set; }
		private EventHubReceiver Receiver { get; set; }

		public async void StartListeningAsync()
		{
			Client = EventHubClient.CreateFromConnectionString(Constants.CharlieActivityEventHubEndpoint,
				Constants.SharedCharlieWarningsHubName);

			ConsumerGroup = Client.GetDefaultConsumerGroup();
			Receiver = ConsumerGroup.CreateReceiver("0", DateTime.Now.ToUniversalTime());

			await Task.Run(StartListeningForWarningCommandsAsync);
		}

		private async Task StartListeningForWarningCommandsAsync()
		{
			while (true)
			{
				await Task.Delay(1);
				try
				{
					var eventData = Receiver.Receive();
					if (eventData == null) continue;
					var bytes = eventData.GetBytes();
					var payload = Encoding.UTF8.GetString(bytes);
					var alert = JsonConvert.DeserializeObject<AlertModel>(payload);
					Messenger.Default.Send(alert);
				}
				catch
				{
					// ignored
				}
			}
		}
	}
}