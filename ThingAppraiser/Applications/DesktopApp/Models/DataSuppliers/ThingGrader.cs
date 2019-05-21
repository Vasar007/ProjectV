using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Crawlers;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Crawlers;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal class ThingGrader : IThingGrader
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ThingGrader>();

        private readonly IImageSupplier _tmdbImageSupplier =
            new TmdbImageSupplier(TmdbServiceConfiguration.Configuration);

        private readonly IImageSupplier _omdbImageSupplier = new OmdbImageSupplier();

        private readonly IImageSupplier _steamImageSupplier = new SteamImageSupplier();


        public ThingGrader()
        {
        }

        #region IThingGrader Implementation

        public List<Thing> ProcessRatings(List<RatingDataContainer> rating)
        {
            _logger.Info("Got rating container to process.");

            if (rating.IsNullOrEmpty()) return new List<Thing>();

            IImageSupplier imageSupplier = DetermineImageSupplier(rating.First().DataHandler);

            List<Thing> result = rating.Select(r =>
                new Thing(
                    Guid.NewGuid(), r.DataHandler,
                    imageSupplier.GetImageLink(r.DataHandler, ImageSize.Large)
                )
            ).ToList();

            _logger.Info("Processing was over.");
            return result;
        }

        public void ProcessMetaData(ResponseMetaData metaData)
        {
            if (metaData.OptionalData.TryGetValue(nameof(TmdbServiceConfiguration),
                                                  out IOptionalData optionalData))
            {
                if (!TmdbServiceConfiguration.HasValue())
                {
                    var tmdbServiceConfig = (TmdbServiceConfigurationInfo) optionalData;
                    TmdbServiceConfiguration.SetServiceConfigurationIfNeed(tmdbServiceConfig);
                }
            }
        }

        #endregion

        private IImageSupplier DetermineImageSupplier(BasicInfo basicInfo)
        {
            switch (basicInfo)
            {
                case TmdbMovieInfo _:
                    return _tmdbImageSupplier;

                case OmdbMovieInfo _:
                    return _omdbImageSupplier;

                case SteamGameInfo _:
                    return _steamImageSupplier;

                default:
                    var ex = new ArgumentOutOfRangeException(nameof(basicInfo),
                                                             "Got unknown type to process.");
                    _logger.Error(ex, "Got unknown type.");
                    throw ex;
            }
        }
    }
}
