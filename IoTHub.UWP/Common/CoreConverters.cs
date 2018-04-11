using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace IoTHub.UWP.Common
{
	public sealed class StatusToFillConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) =>
			(DeviceStatus) value == DeviceStatus.Ok
				? Application.Current.Resources["AppStatusOkFillBrush"] as SolidColorBrush
				: Application.Current.Resources["AppStatusAtRiskStrokeBrush"] as SolidColorBrush;

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			System.Convert.ToBoolean(value) ? parameter : null;
	}

	public sealed class LocalTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var time = (DateTime)value;

			return time.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) =>
			System.Convert.ToBoolean(value) ? parameter : null;
	}
}