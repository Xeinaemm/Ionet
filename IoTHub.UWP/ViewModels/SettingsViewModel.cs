using System.Windows.Input;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using IoTHub.UWP.Services;

namespace IoTHub.UWP.ViewModels
{
	public class SettingsViewModel : ViewModelBase
	{
		private ElementTheme _elementTheme = ThemeSelectorService.Theme;

		private ICommand _switchThemeCommand;

		public ElementTheme ElementTheme
		{
			get => _elementTheme;

			set => Set(ref _elementTheme, value);
		}

		public ICommand SwitchThemeCommand =>
			_switchThemeCommand ?? (_switchThemeCommand = new RelayCommand<ElementTheme>(
				async param =>
				{
					ElementTheme = param;
					await ThemeSelectorService.SetThemeAsync(param);
				}));
	}
}