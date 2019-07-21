using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistItem : ModelBase
    {
        private readonly ToplistBlock _parentBlock;

        private int? _position;

        private string _name;

        public int? Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
                _parentBlock.DeleteItemIfNeed(this);
            }
        }

        public ICommand AddOrUpdateCommand => new RelayCommand(AddOrUpdate);

        public ICommand RemoveCommand => new RelayCommand(Remove);


        public ToplistItem(int? position, string name, ToplistBlock parentBlock)
        {
            // Need to initialize parent block first.
            _parentBlock = parentBlock.ThrowIfNull(nameof(parentBlock));
            Position = position;
            Name = name;
        }

        public ToplistItem Clone()
        {
            return new ToplistItem(Position, Name, _parentBlock);
        }

        // Methods with cast instead of passing this because I want to refactor this sometimes.
        private static void AddOrUpdate(object obj)
        {
            if (obj is ToplistItem toplistItem)
            {
                if (toplistItem._parentBlock.AddOrUpdateItem(toplistItem))
                {
                    toplistItem.Name = string.Empty;
                }
            }
        }

        private static void Remove(object obj)
        {
            if (obj is ToplistItem toplistItem)
            {
                if (!toplistItem._parentBlock.RemoveItem(toplistItem))
                {
                    toplistItem.Name = string.Empty;
                }
            }
        }
    }
}
