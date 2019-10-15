using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.Toplists;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ToplistEditorViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ToplistEditorViewModel>();

        private readonly IEventAggregator _eventAggregator;

        private ToplistWrapper? _toplistWrapper;

        private ObservableCollection<ToplistBlock> _toplistBlocks;
        public ObservableCollection<ToplistBlock> ToplistBlocks
        {
            get => _toplistBlocks;
            set => SetProperty(ref _toplistBlocks, value.ThrowIfNull(nameof(value)));
        }

        public ICommand AddBlockCommand { get; }

        public ICommand RemoveBlockCommand { get; }

        public ICommand SaveToplistCommand { get; }


        public ToplistEditorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            _eventAggregator.GetEvent<LoadToplistFileMessage>().Subscribe(
                ProcessLoadingLocalToplistFile
            );

            _eventAggregator.GetEvent<ConstructToplistMessage>().Subscribe(
                ProcessConstructingNewToplist
            );

            _toplistBlocks = new ObservableCollection<ToplistBlock>();

            AddBlockCommand = new DelegateCommand(AddNewBlock);
            RemoveBlockCommand = new DelegateCommand<ToplistBlock>(RemoveBlock);
            SaveToplistCommand = new DelegateCommand(SaveToplistToFileCommand);
        }

        private void AddNewBlock()
        {
            if (_toplistWrapper is null) return;

            _toplistWrapper.AddNewBlock();
        }

        private void RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            if (_toplistWrapper is null) return;

            _toplistWrapper.RemoveBlock(block);
        }

        private void SaveToplistToFileCommand()
        {
            if (_toplistWrapper is null) return;

            string? filename = ExecutableDialogs.ExecuteSaveToplistFileDialog();
            if (string.IsNullOrWhiteSpace(filename))
            {
                _logger.Info("Skipping saving toplist because got an empty filename value.");
                return;
            }

            ProcessToplistSaving(filename);
        }

        private void ProcessToplistSaving(string toplistFilename)
        {
            if (_toplistWrapper is null) return;

            try
            {
                _toplistWrapper.SaveToplist(toplistFilename);

                MessageBoxProvider.ShowInfo("Toplist was saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist saving.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }

        private void ProcessLoadingLocalToplistFile(string toplistFilename)
        {
            try
            {
                LoadToplist(toplistFilename);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist loading.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }

        private void LoadToplist(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            ToplistBase toplist = ToplistFactory.LoadFromFile(toplistFilename);
            _toplistWrapper = new ToplistWrapper(toplist);
            ToplistBlocks = _toplistWrapper.Blocks;
        }

        private void ProcessConstructingNewToplist(ToplistParametersInfo parameters)
        {
            try
            {
                ConstructNewToplist(
                    parameters.ToplistName, parameters.ToplistType, parameters.ToplistFormat
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist creation.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }

        private void ConstructNewToplist(string toplistName, ToplistType toplistType,
          ToplistFormat toplistFormat)
        {
            toplistName.ThrowIfNullOrEmpty(nameof(toplistName));

            ToplistBase toplist = ToplistFactory.Create(toplistName, toplistType, toplistFormat);
            _toplistWrapper = new ToplistWrapper(toplist);
            ToplistBlocks = _toplistWrapper.Blocks;

            AddNewBlock();
        }
    }
}
