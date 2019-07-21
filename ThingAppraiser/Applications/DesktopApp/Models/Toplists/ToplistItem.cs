using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistItem : ModelBase
    {
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
                ParentBlock.DeleteItemIfNeed(this);
            }
        }

        public ICommand AddOrUpdateCommand => new RelayCommand(AddOrUpdate);

        public ICommand RemoveCommand => new RelayCommand(Remove);

        public ToplistBlock ParentBlock { get; }


        public ToplistItem(int? position, string name, ToplistBlock parentBlock)
        {
            // Need to initialize parent block first.
            ParentBlock = parentBlock.ThrowIfNull(nameof(parentBlock));
            Position = position;
            Name = name;
        }

        public ToplistItem Clone()
        {
            return new ToplistItem(Position, Name, ParentBlock);
        }

        private void AddOrUpdate(object obj)
        {
            if (ParentBlock.AddOrUpdateItem(this))
            {
                Name = string.Empty;
            }
        }

        private void Remove(object obj)
        {
            if (!ParentBlock.RemoveItem(this))
            {
                Name = string.Empty;
            }
        }
    }
}
