using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Linq;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Models.WebServices.Responses;
using ProjectV.TmdbService;

namespace ProjectV.DesktopApp.Models.DataSuppliers
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

            if (rating.IsNullOrEmpty())
            {
                _logger.Info("Rating does not contain any results.");
                return new List<Thing>();
            }

            IImageSupplier imageSupplier = DetermineImageSupplier(rating[0].DataHandler);

            IReadOnlyList<Thing> result = rating.Select(r =>
                new Thing(
                    Guid.NewGuid(), r.DataHandler,
                    imageSupplier.GetImageLink(r.DataHandler, ImageSize.Large)
                )
            ).ToList();

            _logger.Info($"Processing was over. Got {result.Count.ToString()} things.");
            return result;
        }

        public void ProcessMetadata(ProcessingResponseMetadata metadata)
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

        private static IImageSupplier DetermineImageSupplier(BasicInfo basicInfo)
        {
            basicInfo.ThrowIfNull(nameof(basicInfo));

            return basicInfo switch
            {
                OmdbMovieInfo _ => new OmdbImageSupplier(),

                TmdbMovieInfo _ => new TmdbImageSupplier(TmdbServiceConfiguration.Configuration),

                SteamGameInfo _ => new SteamImageSupplier(),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(basicInfo), basicInfo,
                    $"Got unknown type [{basicInfo.GetType().Name}] to process."
                )
            };
        }
    }
}
