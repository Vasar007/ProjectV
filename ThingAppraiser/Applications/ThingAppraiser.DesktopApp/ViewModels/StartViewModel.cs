using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acolyte.Assertions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.Configuration;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.Things;
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

        public ICommand OpenThingsFileDialogCommand { get; }


        public StartViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            SelectedService = AvailableBeautifiedServices.First();

            OpenThingsFileDialogCommand = new DelegateCommand(SendOpenThingsFileMessage);
        }

        private void SendOpenThingsFileMessage()
        {
            string? filename = ExecutableDialogs.ExecuteOpenThingsFileDialog();
            if (string.IsNullOrWhiteSpace(filename))
            {
                _logger.Info("Skipping openning things file because got an empty filename value.");
                return;
            }

            var thingsData = ThingsDataToAppraise.Create(DataSource.LocalFile, filename);
            _eventAggregator
                .GetEvent<AppraiseLocalThingsFileMessage>()
                .Publish(thingsData);
        }
    }
}
