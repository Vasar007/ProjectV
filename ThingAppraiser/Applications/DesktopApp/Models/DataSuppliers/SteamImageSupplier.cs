using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal class SteamImageSupplier : IImageSupplier
    {
        public SteamImageSupplier()
        {
        }

        #region IImageSupplier Implementation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            data.ThrowIfNull(nameof(data));

            if (!(data is SteamGameInfo gameInfo))
            {
                throw new ArgumentException("Data handler has invalid type.", nameof(data));
            }

            return gameInfo.PosterPath;
        }

        #endregion
    }
}
