using System.Windows.Controls;
using Acolyte.Assertions;
using ProjectV.DesktopApp.Models.Toplists;

namespace ProjectV.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistBoxView.xaml
    /// </summary>
    public sealed partial class ToplistBoxView : UserControl
    {
        public ToplistBoxView()
        {
            InitializeComponent();
        }

        internal ToplistBoxView(ToplistItem dataContext)
            : this()
        {
            // We use only this ctor but cannot make it public because of
            // ToplistItem accessibility level.
            DataContext = dataContext.ThrowIfNull(nameof(dataContext));
        }
    }
}
