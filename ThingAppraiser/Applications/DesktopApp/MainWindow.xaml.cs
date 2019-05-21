using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
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

            DataContext = new MainWindowViewModel(MainWindowDialogHost.Identifier);

            _logger.Info("Client application started.");
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Until we had a StaysOpen glag to Drawer, this will help with scroll bars.
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
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
