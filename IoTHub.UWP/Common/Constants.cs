using Windows.Devices.Geolocation;

namespace IoTHub.UWP.Common
{
	public class Constants
	{
		public const string CharlieActivityEventHubEndpoint = "";

		public const string CharlieActivityEventHubName = "";

		public const string DeviceMessagingConnectionString = "";

		public const string SharedCharlieWarningsHubName = "";

		public static BasicGeoposition DefaultStartingLocation = new BasicGeoposition
		{
			Latitude = 52.25114079999999,
			Longitude = 21.00723270000003
		};

		public static Geopoint AreaCenter => new Geopoint(DefaultStartingLocation);

		public const string MapServiceToken = "";

		//public const double AreaRadius = 80467.0;
		public const double InitialZoomLevel = 17.0;
	}
}
