using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using IoTHub.UWP.Common;
using IoTHub.UWP.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace IoTHub.UWP.Views
{
	public sealed partial class MapPage
	{
		public MapPage()
		{
			InitializeComponent();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			ViewModel.Initialize(Map);
			EnsureMapView();
		}

		private MapViewModel ViewModel => DataContext as MapViewModel;

		private async void EnsureMapView()
		{
			await Task.Delay(2000);

			var toolbar = Map.FindDescendant<StackPanel>();
			if (toolbar != null)
				toolbar.Visibility = Visibility.Visible;

			Map.Style = MapStyle.Aerial3DWithRoads;
			await Map.TrySetSceneAsync(MapScene.CreateFromLocation(Constants.AreaCenter));
			Map.ZoomLevel = Constants.InitialZoomLevel;
		}
	}
}