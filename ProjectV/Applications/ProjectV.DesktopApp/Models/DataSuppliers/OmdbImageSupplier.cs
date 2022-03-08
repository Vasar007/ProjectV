﻿using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class OmdbImageSupplier : IImageSupplier
    {
        public OmdbImageSupplier()
        {
        }

        #region IImageSupplier Implementation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            data.ThrowIfNull(nameof(data));

            if (data is not OmdbMovieInfo movieInfo)
            {
                throw new ArgumentException("Data handler has invalid type.", nameof(data));
            }

            return movieInfo.PosterPath;
        }

        #endregion
    }
}
