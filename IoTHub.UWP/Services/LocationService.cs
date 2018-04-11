using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using IoTHub.UWP.Helpers;

namespace IoTHub.UWP.Services
{
	public class LocationService
	{
		private Geolocator _geolocator;

		public Geoposition CurrentPosition { get; private set; }

		public event EventHandler<Geoposition> PositionChanged;

		public Task<bool> InitializeAsync() => InitializeAsync(100);

		public Task<bool> InitializeAsync(uint desiredAccuracyInMeters) =>
			InitializeAsync(desiredAccuracyInMeters, (double) desiredAccuracyInMeters / 2);

		private async Task<bool> InitializeAsync(uint desiredAccuracyInMeters, double movementThreshold)
		{
			// to find out more about getting location, go to https://docs.microsoft.com/en-us/windows/uwp/maps-and-location/get-location
			if (_geolocator != null)
			{
				_geolocator.PositionChanged -= Geolocator_PositionChanged;
				_geolocator = null;
			}

			var access = await Geolocator.RequestAccessAsync();

			bool result;

			switch (access)
			{
				case GeolocationAccessStatus.Allowed:
					_geolocator = new Geolocator
					{
						DesiredAccuracyInMeters = desiredAccuracyInMeters,
						MovementThreshold = movementThreshold
					};
					result = true;
					break;
				default:
					result = false;
					break;
			}

			return result;
		}

		public async Task StartListeningAsync()
		{
			if (_geolocator == null)
				throw new InvalidOperationException("ExceptionLocationServiceStartListeningCanNotBeCalled".GetLocalized());

			_geolocator.PositionChanged += Geolocator_PositionChanged;

			CurrentPosition = await _geolocator.GetGeopositionAsync();
		}

		public void StopListening()
		{
			if (_geolocator == null) return;

			_geolocator.PositionChanged -= Geolocator_PositionChanged;
		}

		private async void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
		{
			if (args == null) return;

			CurrentPosition = args.Position;

			await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
				() => { PositionChanged?.Invoke(this, CurrentPosition); });
		}
	}
}