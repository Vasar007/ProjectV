using System.Windows.Controls;
using Prism.Mvvm;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for BrowsingView.xaml
    /// </summary>
    public sealed partial class BrowsingView : UserControl
    {
        public BrowsingView(BindableBase dataContext)
        {
            InitializeComponent();

            DataContext = dataContext.ThrowIfNull(nameof(dataContext));
        }
    }
}
