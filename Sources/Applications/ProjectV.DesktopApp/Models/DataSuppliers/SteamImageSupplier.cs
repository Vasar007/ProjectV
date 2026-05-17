using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class SteamImageSupplier : IImageSupplier
    {
        public SteamImageSupplier()
        {
        }

        #region IImageSupplier Implementation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            data.ThrowIfNull(nameof(data));

            if (data is not SteamGameInfo gameInfo)
            {
                throw new ArgumentException("Data handler has invalid type.", nameof(data));
            }

            return gameInfo.PosterPath;
        }

        #endregion
    }
}
