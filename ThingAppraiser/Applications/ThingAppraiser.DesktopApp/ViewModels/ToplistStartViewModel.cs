using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ToplistStartViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ToplistStartViewModel>();

        private readonly IEventAggregator _eventAggregator;

        public object DialogIdentifier { get; }

        public ICommand CreateToplistDialogCommand { get; }

        public ICommand OpenToplistDialogCommand { get; }


        public ToplistStartViewModel(object dialogIdentifier, IEventAggregator eventAggregator)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            CreateToplistDialogCommand = new DelegateCommand<ToplistStartViewModel>(
                ExecutableDialogs.ExecuteCreateToplistDialog
            );
            OpenToplistDialogCommand = new DelegateCommand(SendOpenToplistFileMessage);
        }

        private void SendOpenToplistFileMessage()
        {
            string? filename = ExecutableDialogs.ExecuteOpenToplistFileDialog();
            if (string.IsNullOrWhiteSpace(filename))
            {
                _logger.Info("Skipping openning toplist because got an empty filename value.");
                return;
            }

            _eventAggregator.GetEvent<OpenToplistFileMessage>().Publish(filename);
        }
    }
}
