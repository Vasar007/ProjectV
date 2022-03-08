﻿using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.TmdbService;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class TmdbImageSupplier : IImageSupplier
    {
        private TmdbServiceConfigurationInfo _serviceConfigurationInfo;


        public TmdbImageSupplier(TmdbServiceConfigurationInfo serviceConfigurationInfo)
        {
            _serviceConfigurationInfo =
                serviceConfigurationInfo.ThrowIfNull(nameof(serviceConfigurationInfo));
        }

        #region IImageSupplier Implementation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            data.ThrowIfNull(nameof(data));

            if (_serviceConfigurationInfo is null)
            {
                _serviceConfigurationInfo = TmdbServiceConfiguration.Configuration;
            }

            if (data is not TmdbMovieInfo movieInfo)
            {
                throw new ArgumentException("Data handler has invalid type.", nameof(data));
            }

            int sizeIndex = GetImageSizeIndex(imageSize,
                                              _serviceConfigurationInfo.PosterSizes.Count);

            string imageSizeValue = _serviceConfigurationInfo.PosterSizes[sizeIndex];


            string result = _serviceConfigurationInfo.SecureBaseUrl + imageSizeValue +
                            movieInfo.PosterPath;
            return result;
        }

        #endregion

        private static int GetImageSizeIndex(ImageSize imageSize, int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                                                      "Length value must be positive.");
            }

            return imageSize switch
            {
                ImageSize.Small => 0,

                ImageSize.Middle => length / 3,

                ImageSize.Large => length * 2 / 3,

                ImageSize.Origin => length - 1,

                _ => throw new ArgumentOutOfRangeException(nameof(imageSize), imageSize,
                                                           "Invalid image size value.")
            };
        }
    }
}
