using System;
using System.Windows;
using System.Windows.Input;
using DesktopApp.View;
using DesktopApp.ViewModel;
using ThingAppraiser.Logging;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CMainWindow : Window
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CMainWindow>();

        public CMainWindow()
        {
            InitializeComponent();

            DataContext = new CMainWindowViewModel(new CStartControl());

            s_logger.Info("Client application started.");
        }

        private void OnCopy(Object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is String stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex, "Data couldn't be copied to clipboard.");
                }
            }
        }
    }
}
