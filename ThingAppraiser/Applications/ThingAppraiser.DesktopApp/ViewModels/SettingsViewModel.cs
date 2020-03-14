using System;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
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

            IBaseTheme newTheme = isDark.Value ? Theme.Dark : Theme.Light;
            ModifyTheme(theme => theme.SetBaseTheme(newTheme));
        }

        private static void ModifyTheme(Action<ITheme>? modificationAction)
        {
            _logger.Info("Modifying application theme.");

            PaletteHelper paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            modificationAction?.Invoke(theme);

            paletteHelper.SetTheme(theme);
        }
    }
}
