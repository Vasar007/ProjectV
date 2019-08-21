using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ThingAppraiser.Core.Building;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class StartViewModel : ViewModelBase
    {
        private string _selectedService;

        public IReadOnlyList<string> AvailableBeautifiedServices { get; } =
            ConfigContract.AvailableBeautifiedServices;

        public string SelectedService
        {
            get => _selectedService;
            set => SetProperty(ref _selectedService, value.ThrowIfNull(nameof(value)));
        }

        public object DialogIdentifier { get; }

        public ICommand InputThingDialogCommand =>
            new RelayCommand<StartViewModel>(ExecutableDialogs.ExecuteInputThingDialog);

        public ICommand OpenFileDialogCommand =>
            new RelayCommand<MainWindowViewModel>(ExecutableDialogs.ExecuteOpenThingsFileDialog);

        public ICommand EnterDataDialogCommand =>
            new RelayCommand<StartViewModel>(ExecutableDialogs.ExecuteEnterDataDialog);


        public StartViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            SelectedService = AvailableBeautifiedServices.First();
        }
    }
}
