using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal class Thing : ModelBase
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
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public int ThingId
        {
            get => _thingId;
            set => SetProperty(ref _thingId, value);
        }

        public double VoteAverage
        {
            get => _voteAverage;
            set => SetProperty(ref _voteAverage, value);
        }

        public int VoteCount
        {
            get => _voteCount;
            set => SetProperty(ref _voteCount, value);
        }

        public string ImageLink
        {
            get => _imageLink;
            set => SetProperty(ref _imageLink, value.ThrowIfNull(nameof(value)));
        }


        public Thing(Guid internalId, BasicInfo data, string imageLink)
        {
            InternalId = internalId.ThrowIfEmpty(nameof(internalId));
            UpdateData(data);
            ImageLink = imageLink;
        }

        public void UpdateData(BasicInfo data)
        {
            data.ThrowIfNull(nameof(data));

            Name = data.Title;
            ThingId = data.ThingId;
            VoteAverage = data.VoteAverage;
            VoteCount = data.VoteCount;
        }
    }
}
