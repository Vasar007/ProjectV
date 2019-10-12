using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.Configuration;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class StartViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ToplistEditorViewModel>();

        private readonly IEventAggregator _eventAggregator;

        public IReadOnlyList<string> AvailableBeautifiedServices { get; } =
            ConfigContract.AvailableBeautifiedServices;

        private string _selectedService = default!; // Initializes throught property.
        public string SelectedService
        {
            get => _selectedService;
            set => SetProperty(ref _selectedService, value.ThrowIfNull(nameof(value)));
        }

        public object DialogIdentifier { get; }

        public ICommand InputThingDialogCommand { get; }

        public ICommand OpenThingsFileDialogCommand { get; }

        public ICommand EnterDataDialogCommand { get; }


        public StartViewModel(IEventAggregator eventAggregator)
        {
            DialogIdentifier = MainDialogIdentifier.DialogIdentifier;
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            SelectedService = AvailableBeautifiedServices.First();

            InputThingDialogCommand =
                new DelegateCommand<StartViewModel>(ExecutableDialogs.ExecuteInputThingDialog);

            OpenThingsFileDialogCommand = new DelegateCommand(SendOpenThingsFileMessage);

            EnterDataDialogCommand =
                new DelegateCommand<StartViewModel>(ExecutableDialogs.ExecuteEnterDataDialog);
        }

        private void SendOpenThingsFileMessage()
        {
            string? filename = ExecutableDialogs.ExecuteOpenThingsFileDialog();
            if (string.IsNullOrWhiteSpace(filename))
            {
                _logger.Info("Skipping openning things file because got an empty filename value.");
                return;
            }

            _eventAggregator.GetEvent<OpenThingsFileMessage>().Publish(filename);
        }
    }
}
