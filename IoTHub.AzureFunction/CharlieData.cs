using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IoTHub.AzureFunction
{
	public static class CharlieData
	{
		private const double Latitude = 52.25114079999999;
		private const double Longitude = 21.00723270000003;

		[FunctionName("CharlieFunction")]
		public static async Task Run(
			[EventHubTrigger("", Connection = "IoTHubConnection")]
			string inputMessage,
			[EventHub("outeventhub", Connection = "EventHubConnection")]
			IAsyncCollector<string> outputMessage,
			[EventHub("sharedouteventhub", Connection = "SharedEventHubConnection")]
			IAsyncCollector<string> sharedOutputMessage,
			TraceWriter log)
		{
			var converter = new IsoDateTimeConverter {DateTimeFormat = "dd/MM/yy HH:mm:ss"};
			var input = JsonConvert.DeserializeObject<Input>(inputMessage, converter);
			if (input == null)
			{
				log.Info("Deserialization fault");
				return;
			}

			var hash = (uint) input.DeviceId.GetHashCode();
			var lat = Latitude + hash % 100 / 1000000.0;
			var lng = Longitude + hash % 100 / 1000000.0;

			var output = new Output
			{
				DeviceId = input.DeviceId,
				Timestamp = input.Timestamp,
				Temperature = input.Temperature,
				Humidity = input.Humidity,
				Latitude = lat,
				Longitude = lng,
				X = input.X,
				Y = input.Y,
				Z = input.Z
			};
			var outputPayload = JsonConvert.SerializeObject(output);

			await outputMessage.AddAsync(outputPayload);
			await sharedOutputMessage.AddAsync(outputPayload);
			log.Info($"DeviceId={input.DeviceId}");
		}
	}

	internal class Input
	{
#pragma warning disable 649
		public string DeviceId;
		public double Humidity;
#pragma warning disable 169
		public string MessageId;
		public double Temperature;
		public DateTime Timestamp;
		public double X;
		public double Y;
		public double Z;
#pragma warning restore 169
#pragma warning restore 649
	}

	internal class Output
	{
#pragma warning disable 414
		public string DeviceId;
		public double Humidity;
		public double Latitude;
		public double Longitude;
		public double Temperature;
		public DateTime Timestamp;
		public double X;
		public double Y;
		public double Z;
#pragma warning restore 414
	}
}