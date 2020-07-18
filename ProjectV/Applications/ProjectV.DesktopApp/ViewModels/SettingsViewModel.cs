using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using ProjectV.Logging;

namespace ProjectV.DesktopApp.ViewModels
{
    internal sealed class SettingsViewModel : BindableBase
    {
        // TODO: allow to save into config all setting values.

        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SettingsViewModel>();

        public ICommand ToggleBaseCommand { get; }


        public SettingsViewModel()
        {
            ToggleBaseCommand = new DelegateCommand<bool?>(ApplyBase);
        }

        private static void ApplyBase(bool? isDark)
        {
            if (!isDark.HasValue)
            {
                throw new ArgumentException("Boolean flag should be specified.", nameof(isDark));
            }

            IBaseTheme newTheme;
            string newThemeName;
            if (isDark.Value)
            {
                newTheme = Theme.Dark;
                newThemeName = "Dark";
            }
            else
            {
                newTheme = Theme.Light;
                newThemeName = "Light";
            }

            _logger.Info($"Changing application theme. New theme: '{newThemeName}'.");

            ModifyTheme(theme => theme.SetBaseTheme(newTheme));
        }

        private static void ModifyTheme(Action<ITheme>? modificationAction)
        {
            _logger.Info("Modifying application theme.");

            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
