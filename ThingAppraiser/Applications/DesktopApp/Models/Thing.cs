using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThingAppraiser.Data;

namespace ThingAppraiser.DesktopApp.Models
{
    internal class Thing : INotifyPropertyChanged
    {
        private string _name;

        private int _thingId;

        private double _voteAverage;

        private int _voteCount;

        private string _imageLink;

        public Guid InternalId { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int ThingId
        {
            get => _thingId;
            set
            {
                _thingId = value;
                OnPropertyChanged();
            }
        }

        public double VoteAverage
        {
            get => _voteAverage;
            set
            {
                _voteAverage = value;
                OnPropertyChanged();
            }
        }

        public int VoteCount
        {
            get => _voteCount;
            set
            {
                _voteCount = value;
                OnPropertyChanged();
            }
        }

        public string ImageLink
        {
            get => _imageLink;
            set
            {
                _imageLink = value;
                OnPropertyChanged();
            }
        }


        public Thing(Guid internalId, BasicInfo data, string imageLink)
        {
            InternalId = internalId;
            UpdateData(data);
            ImageLink = imageLink;
        }

        public void UpdateData(BasicInfo data)
        {
            Name = data.Title;
            ThingId = data.ThingId;
            VoteAverage = data.VoteAverage;
            VoteCount = data.VoteCount;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
