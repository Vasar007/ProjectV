using System.Windows.Input;
using Prism.Commands;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal sealed class ToplistItem : ModelBase
    {
        private string _name = default!; // Initializes throught property.
        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
                ParentBlock.DeleteItemIfNeed(this);
            }
        }

        private int? _position;
        public int? Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public ICommand AddOrUpdateItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand AddOrUpdateItemByEnterHitCommand { get; }

        public ToplistBlock ParentBlock { get; }


        public ToplistItem(string name, int? position, ToplistBlock parentBlock)
        {
            // Need to initialize parent block first.
            ParentBlock = parentBlock.ThrowIfNull(nameof(parentBlock));
            Name = name;
            Position = position;

            AddOrUpdateItemCommand = new DelegateCommand(AddOrUpdateItem);
            RemoveItemCommand = new DelegateCommand(RemoveItem);
            AddOrUpdateItemByEnterHitCommand =
                new DelegateCommand<string>(AddOrUpdateItemByEnterHit);
        }

        public ToplistItem Clone()
        {
            return new ToplistItem(Name, Position, ParentBlock);
        }

        private void AddOrUpdateItem()
        {
            if (ParentBlock.AddOrUpdateItem(this))
            {
                Name = string.Empty;
            }
        }

        private void RemoveItem()
        {
            if (!ParentBlock.RemoveItem(this))
            {
                Name = string.Empty;
            }
        }

        private void AddOrUpdateItemByEnterHit(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                RemoveItem();
            }

            Name = newName;
            AddOrUpdateItem();
        }
    }
}
