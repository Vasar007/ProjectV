using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThingAppraiser.Data;

namespace DesktopApp.Model
{
    public class CThing : INotifyPropertyChanged
    {
        private String _name;

        private Int32 _thingID;

        private Single _voteAverage;

        private Int32 _voteCount;

        private String _imageLink;

        public Guid InternalID { get; }

        public String Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public Int32 ThingID
        {
            get => _thingID;
            set
            {
                _thingID = value;
                OnPropertyChanged();
            }
        }

        public Single VoteAverage
        {
            get => _voteAverage;
            set
            {
                _voteAverage = value;
                OnPropertyChanged();
            }
        }

        public Int32 VoteCount
        {
            get => _voteCount;
            set
            {
                _voteCount = value;
                OnPropertyChanged();
            }
        }

        public String ImageLink
        {
            get => _imageLink;
            set
            {
                _imageLink = value;
                OnPropertyChanged();
            }
        }


        public CThing(Guid internalID, CBasicInfo data, String imageLink)
        {
            InternalID = internalID;
            UpdateData(data);
            ImageLink = imageLink;
        }

        public void UpdateData(CBasicInfo data)
        {
            Name = data.Title;
            ThingID = data.ID;
            VoteAverage = data.VoteAverage;
            VoteCount = data.VoteCount;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
