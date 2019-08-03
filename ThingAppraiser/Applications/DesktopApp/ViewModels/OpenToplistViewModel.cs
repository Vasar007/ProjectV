namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class OpenToplistViewModel : ViewModelBase
    {
        public object DialogIdentifier { get; }


        public OpenToplistViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
