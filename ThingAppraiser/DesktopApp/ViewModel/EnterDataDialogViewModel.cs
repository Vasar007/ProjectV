using System;

namespace DesktopApp.ViewModel
{
    public class CEnterDataDialogViewModel : CViewModelBase
    {
        private String _name;

        public String Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public CEnterDataDialogViewModel()
        {
        }
    }
}
