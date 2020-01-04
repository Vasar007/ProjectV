using System.Windows.Controls;
using Acolyte.Assertions;
using Prism.Mvvm;

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
