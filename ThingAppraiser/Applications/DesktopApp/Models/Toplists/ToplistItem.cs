using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistItem : ModelBase
    {
        private string _name;

        private int? _position;

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
                ParentBlock.DeleteItemIfNeed(this);
            }
        }

        public int? Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public ICommand AddOrUpdateCommand => new RelayCommand(AddOrUpdate);

        public ICommand RemoveCommand => new RelayCommand(Remove);

        public ICommand AddOrUpdateByEnterHitCommand =>
            new RelayCommand<string>(AddOrUpdateByEnterHit);

        public ToplistBlock ParentBlock { get; }


        public ToplistItem(string name, int? position,ToplistBlock parentBlock)
        {
            // Need to initialize parent block first.
            ParentBlock = parentBlock.ThrowIfNull(nameof(parentBlock));
            Name = name;
            Position = position;
        }

        public ToplistItem Clone()
        {
            return new ToplistItem(Name, Position, ParentBlock);
        }

        private void AddOrUpdate()
        {
            if (ParentBlock.AddOrUpdateItem(this))
            {
                Name = string.Empty;
            }
        }

        private void Remove()
        {
            if (!ParentBlock.RemoveItem(this))
            {
                Name = string.Empty;
            }
        }

        private void AddOrUpdateByEnterHit(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                Remove();
            }

            Name = newName;
            AddOrUpdate();
        }
    }
}
