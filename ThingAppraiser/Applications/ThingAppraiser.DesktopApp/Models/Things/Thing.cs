using System;
using Prism.Mvvm;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal sealed class Thing : BindableBase
    {
        public Guid InternalId { get; }

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        private int _thingId;
        public int ThingId
        {
            get => _thingId;
            set => SetProperty(ref _thingId, value);
        }

        private double _voteAverage;
        public double VoteAverage
        {
            get => _voteAverage;
            set => SetProperty(ref _voteAverage, value);
        }

        private int _voteCount;
        public int VoteCount
        {
            get => _voteCount;
            set => SetProperty(ref _voteCount, value);
        }

        private string _imageLink = default!; // Initializes throught property.
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
