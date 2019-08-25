using System;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.TmdbService;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal sealed class TmdbImageSupplier : IImageSupplier
    {
        private TmdbServiceConfigurationInfo _serviceConfigurationInfo;


        public TmdbImageSupplier(TmdbServiceConfigurationInfo serviceConfigurationInfo)
        {
            _serviceConfigurationInfo =
                serviceConfigurationInfo.ThrowIfNull(nameof(serviceConfigurationInfo));
        }

        #region IImageSupplier Implamentation

        public string GetImageLink(BasicInfo data, ImageSize imageSize)
        {
            data.ThrowIfNull(nameof(data));

            if (_serviceConfigurationInfo is null)
            {
                _serviceConfigurationInfo = TmdbServiceConfiguration.Configuration;
            }
            _serviceConfigurationInfo.ThrowIfNull(nameof(_serviceConfigurationInfo));

            if (!(data is TmdbMovieInfo movieInfo))
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

            switch (imageSize)
            {
                case ImageSize.Small:
                {
                    return 0;
                }

                case ImageSize.Middle:
                {
                    return length / 3;
                }

                case ImageSize.Large:
                {
                    return length * 2 / 3;
                }

                case ImageSize.Origin:
                {
                    return length - 1;
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(imageSize), imageSize,
                                                          "Invalid image size value.");
                }
            }
        }
    }
}
