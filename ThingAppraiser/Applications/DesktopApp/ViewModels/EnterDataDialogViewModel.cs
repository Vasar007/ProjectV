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


        public EnterDataDialogViewModel()
        {
        }
    }
}
