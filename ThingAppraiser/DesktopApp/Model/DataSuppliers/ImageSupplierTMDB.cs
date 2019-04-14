using System;
using ThingAppraiser;
using ThingAppraiser.Crawlers;
using ThingAppraiser.Data;

namespace DesktopApp.Model.DataSuppliers
{
    public class CImageSupplierTMDB : IImageSupplier
    {
        private CServiceConfigurationTMDB _serviceConfiguration;


        public CImageSupplierTMDB(CServiceConfigurationTMDB serviceConfiguration)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        #region IImageSupplier Implamentation

        public String GetImageLink(CBasicInfo data, EImageSize imageSize)
        {
            if (_serviceConfiguration is null)
            {
                _serviceConfiguration = CCrawlerTMDB.ServiceConfigurationTMDB;
            }

            _serviceConfiguration.ThrowIfNull(nameof(_serviceConfiguration));

            if (!(data is CMovieTMDBInfo movieInfo))
            {
                throw new ArgumentException(@"Data handler has invalid type.", nameof(data));
            }

            Int32 sizeIndex = GetImageSizeIndex(imageSize,
                                                _serviceConfiguration.PosterSizes.Count);

            String imageSizeValue = _serviceConfiguration.PosterSizes[sizeIndex];


            String result = _serviceConfiguration.SecureBaseUrl + imageSizeValue +
                            movieInfo.PosterPath;
            return result;
        }

        #endregion

        private static Int32 GetImageSizeIndex(EImageSize imageSize, Int32 length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                                                      @"Length value must be positive.");
            }

            switch (imageSize)
            {
                case EImageSize.Small:
                    return 0;

                case EImageSize.Middle:
                    return length / 3;

                case EImageSize.Large:
                    return length * 2 / 3;

                case EImageSize.Origin:
                    return length - 1;

                default:
                    throw new ArgumentOutOfRangeException(nameof(imageSize), imageSize,
                                                          @"Invalid image size value.");
            }
        }
    }
}
