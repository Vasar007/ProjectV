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

        public bool IsDark => GetIsDarkValue();


        public SettingsViewModel()
        {
            ToggleBaseCommand = new DelegateCommand<bool?>(ApplyBase);
        }

        public bool GetIsDarkValue()
        {
            var currentTheme = Theme.GetSystemTheme();
            return currentTheme is BaseTheme.Dark;
        }

        private static void ApplyBase(bool? isDark)
        {
            if (!isDark.HasValue)
            {
                throw new ArgumentException("Boolean flag should be specified.", nameof(isDark));
            }

            BaseTheme newTheme;
            if (isDark.Value)
            {
                newTheme = BaseTheme.Dark;
            }
            else
            {
                newTheme = BaseTheme.Light;
            }

            _logger.Info($"Changing application theme. New theme: '{newTheme}'.");

            ModifyTheme(theme => theme.SetBaseTheme(newTheme));
        }

        private static void ModifyTheme(Action<Theme>? modificationAction)
        {
            _logger.Info("Modifying application theme.");

            var paletteHelper = new PaletteHelper();
            Theme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
