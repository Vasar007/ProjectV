namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class EnterDataViewModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public string HintText { get; }
        

        public EnterDataViewModel(string hintText)
        {
            HintText = hintText.ThrowIfNullOrEmpty(nameof(hintText));
        }
    }
}
