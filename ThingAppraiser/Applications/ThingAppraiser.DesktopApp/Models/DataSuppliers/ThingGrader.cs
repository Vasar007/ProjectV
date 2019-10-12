using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.TmdbService;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal sealed class ThingGrader : IThingGrader
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ThingGrader>();


        public ThingGrader()
        {
        }

        #region IThingGrader Implementation

        public IReadOnlyList<Thing> ProcessRatings(IReadOnlyList<RatingDataContainer> rating)
        {
            _logger.Info("Got rating container to process.");

            if (rating.IsNullOrEmpty()) return new List<Thing>();

            IImageSupplier imageSupplier = DetermineImageSupplier(rating.First().DataHandler);

            IReadOnlyList<Thing> result = rating.Select(r =>
                new Thing(
                    Guid.NewGuid(), r.DataHandler,
                    imageSupplier.GetImageLink(r.DataHandler, ImageSize.Large)
                )
            ).ToList();

            _logger.Info("Processing was over.");
            return result;
        }

        public void ProcessMetadata(ResponseMetadata metadata)
        {
            metadata.ThrowIfNull(nameof(metadata));

            if (metadata.OptionalData.TryGetValue(nameof(TmdbServiceConfiguration),
                                                  out IOptionalData? optionalData))
            {
                if (optionalData is null)
                {
                    throw new InvalidOperationException($"{nameof(optionalData)} cannot be null.");
                }

                if (!TmdbServiceConfiguration.HasValue)
                {
                    var tmdbServiceConfig = (TmdbServiceConfigurationInfo) optionalData;
                    TmdbServiceConfiguration.SetServiceConfigurationAnyway(tmdbServiceConfig);
                }
            }
        }

        #endregion

        private IImageSupplier DetermineImageSupplier(BasicInfo basicInfo)
        {
            basicInfo.ThrowIfNull(nameof(basicInfo));

            return basicInfo switch
            {
                OmdbMovieInfo _ => new OmdbImageSupplier(),

                TmdbMovieInfo _ => new TmdbImageSupplier(TmdbServiceConfiguration.Configuration),

                SteamGameInfo _ => new SteamImageSupplier(),

                _ => throw new ArgumentOutOfRangeException(nameof(basicInfo), basicInfo,
                                                           "Got unknown type to process.")
            };
        }
    }
}
