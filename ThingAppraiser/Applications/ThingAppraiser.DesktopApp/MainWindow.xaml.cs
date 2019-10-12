using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Prism.Events;
using Prism.Ioc;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<MainWindow>();

        private readonly IContainerExtension _container;


        public MainWindow(IContainerExtension container)
        {
            InitializeComponent();

            _container = container.ThrowIfNull(nameof(container));

            MainDialogIdentifier.SetDialogIdentifierAnyway(MainWindowDialogHost.Identifier);

            var eventAggregator = _container.Resolve<IEventAggregator>();
            DataContext = new MainWindowViewModel(eventAggregator);

            _logger.Info("Main window was created.");
        }

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender,
            MouseButtonEventArgs eventArgs)
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

        private void OnCopy(object sender, ExecutedRoutedEventArgs eventArgs)
        {
            if (eventArgs.Parameter is string stringValue)
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
