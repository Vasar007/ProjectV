using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public sealed partial class StartView : UserControl
    {
        public StartView(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new StartViewModel(dialogIdentifier);
        }
    }
}
