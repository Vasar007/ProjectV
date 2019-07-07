namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class EnterDataDialogViewModel : ViewModelBase
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string HintText { get; }
        

        public EnterDataDialogViewModel(string hintText)
        {
            HintText = hintText.ThrowIfNullOrEmpty(nameof(hintText));
        }
    }
}
