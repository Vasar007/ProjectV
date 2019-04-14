using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DesktopApp.Domain.Commands;
using ThingAppraiser;

namespace DesktopApp.ViewModel
{
    public class CInputThingViewModel : CViewModelBase
    {
        private ObservableCollection<String> _thingList = new ObservableCollection<String>();

        private String _thingName;

        public ObservableCollection<String> ThingList
        {
            get => _thingList;
            set => SetProperty(ref _thingList, value);
        }

        public String ThingName
        {
            get => _thingName;
            set => SetProperty(ref _thingName, value);
        }

        public Object DialogIdentifier { get; }

        public Object DialogContent { get; set; }

        public ICommand EnterThingNameDialogCommand =>
            new CRelayCommand(SExecutableDialogs.ExecuteEnterThingNameDialog);


        public CInputThingViewModel(Object dialogIdentifier, Object dialogContent)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            DialogContent = dialogContent.ThrowIfNull(nameof(dialogContent));
        }
    }
}
