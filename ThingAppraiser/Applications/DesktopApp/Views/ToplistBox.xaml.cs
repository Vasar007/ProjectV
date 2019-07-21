using System.Windows.Controls;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistBox.xaml
    /// </summary>
    public partial class ToplistBox : UserControl
    {
        public ToplistBox()
        {
            InitializeComponent();
        }

        internal ToplistBox(ToplistItem dataContext)
            : this()
        {
            DataContext = dataContext.ThrowIfNull(nameof(dataContext));
        }
    }
}
