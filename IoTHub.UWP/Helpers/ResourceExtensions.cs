using Windows.ApplicationModel.Resources;

namespace IoTHub.UWP.Helpers
{
	internal static class ResourceExtensions
	{
		private static readonly ResourceLoader ResLoader = new ResourceLoader();

		public static string GetLocalized(this string resourceKey) => ResLoader.GetString(resourceKey);
	}
}