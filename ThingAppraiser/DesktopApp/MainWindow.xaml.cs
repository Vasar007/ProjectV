using System;
using System.Windows;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<MainWindow>();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel(new StartControl());

            _logger.Info("Client application started.");
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Data couldn't be copied to clipboard.");
                }
            }
        }
    }
}
