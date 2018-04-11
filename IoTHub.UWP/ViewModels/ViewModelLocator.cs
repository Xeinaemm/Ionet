using GalaSoft.MvvmLight.Ioc;
using IoTHub.UWP.Services;
using IoTHub.UWP.Views;
using Microsoft.Practices.ServiceLocation;

namespace IoTHub.UWP.ViewModels
{
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			SimpleIoc.Default.Register(() => new NavigationServiceEx());
			Register<PivotViewModel, PivotPage>();
			Register<DevicesChartViewModel, DevicesChartPage>();
			Register<DevicesViewModel, DevicesPage>();
			Register<MapViewModel, MapPage>();
			Register<SettingsViewModel, SettingsPage>();
		}

		public SettingsViewModel SettingsViewModel => ServiceLocator.Current.GetInstance<SettingsViewModel>();

		public MapViewModel MapViewModel => ServiceLocator.Current.GetInstance<MapViewModel>();

		public DevicesChartViewModel DevicesChartViewModel => ServiceLocator.Current.GetInstance<DevicesChartViewModel>();

		public DevicesViewModel DevicesViewModel => ServiceLocator.Current.GetInstance<DevicesViewModel>();

		public PivotViewModel PivotViewModel => ServiceLocator.Current.GetInstance<PivotViewModel>();

		public NavigationServiceEx NavigationService => ServiceLocator.Current.GetInstance<NavigationServiceEx>();

		private void Register<TVm, TV>()
			where TVm : class
		{
			SimpleIoc.Default.Register<TVm>();

			NavigationService.Configure(typeof(TVm).FullName, typeof(TV));
		}
	}
}