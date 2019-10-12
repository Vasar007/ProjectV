using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

        private ToplistBase? _toplist;

        private ObservableCollection<ToplistBlock> _toplistBlocks;
        public ObservableCollection<ToplistBlock> ToplistBlocks
        {
            get => _toplistBlocks;
            set => SetProperty(ref _toplistBlocks, value.ThrowIfNull(nameof(value)));
        }

        public ICommand AddOrUpdateBlockCommand { get; }

        public ICommand RemoveBlockCommand { get; }

        public ICommand SaveToplistCommand { get; }


        public ToplistEditorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            _eventAggregator.GetEvent<LoadToplistFileMessage>().Subscribe(
                ProcessLoadingToplistFile
            );

            _eventAggregator.GetEvent<ConstructToplistMessage>().Subscribe(
                ProcessConstructingNewToplist
            );

            _toplistBlocks = new ObservableCollection<ToplistBlock>();

            AddOrUpdateBlockCommand = new DelegateCommand(AddNewBlock);
            RemoveBlockCommand = new DelegateCommand<ToplistBlock>(RemoveBlock);
            SaveToplistCommand = new DelegateCommand(SaveToplistToFileCommand);
        }

        public void ConstructNewToplist(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            toplistName.ThrowIfNullOrEmpty(nameof(toplistName));

            _toplist = ToplistFactory.Create(toplistName, toplistType, toplistFormat);
            ToplistBlocks = _toplist.Blocks;

            AddNewBlock();
        }

        public void LoadToplist(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            _toplist = ToplistFactory.LoadFromFile(toplistFilename);
            ToplistBlocks = _toplist.Blocks;
        }

        public void SaveToplist(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            if (_toplist is null)
            {
                throw new InvalidOperationException(
                    $"Toplist ({nameof(_toplist)}) should be initialized at first."
                );
            }

            string toplistData = ToplistBase.Serialize(_toplist);
            File.WriteAllText(toplistFilename, toplistData);
        }

        private void AddNewBlock()
        {
            if (_toplist is null) return;

            // Find first inconsistent block number in sorted sequence and select it as block number
            // to add.

            IReadOnlyList<int> existsNumbers = _toplist.Blocks
                .Select(block => block.Number)
                .OrderBy(number => number)
                .ToReadOnlyList();

            int blockNumber = existsNumbers.Count > 0 && existsNumbers.First() != 1
                ? 1
                : _toplist.Blocks.Count + 1;

            for (int i = 1; i < existsNumbers.Count; ++i)
            {
                if (existsNumbers[i] - existsNumbers[i - 1] != 1)
                {
                    blockNumber = existsNumbers[i - 1] + 1;
                    break;
                }
            }

            _toplist.AddBlock(new ToplistBlock(blockNumber.ToString(), blockNumber));
        }

        private void RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            if (_toplist is null) return;

            _toplist.RemoveBlock(block);
        }

        private void SaveToplistToFileCommand()
        {
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
            try
            {
                SaveToplist(toplistFilename);

                MessageBoxHelper.ShowInfo("Toplist was saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist saving.");
                MessageBoxHelper.ShowError(ex.Message);
            }
        }

        private void ProcessLoadingToplistFile(string toplistFilename)
        {
            try
            {
                LoadToplist(toplistFilename);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist loading.");
                MessageBoxHelper.ShowError(ex.Message);
            }
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
                MessageBoxHelper.ShowError(ex.Message);
            }
        }
    }
}
