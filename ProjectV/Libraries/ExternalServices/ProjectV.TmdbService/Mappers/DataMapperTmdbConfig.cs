using ProjectV.Models.Internal;
using TMDbLib.Objects.General;

namespace ProjectV.TmdbService.Mappers
{
    public sealed class DataMapperTmdbConfig : IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo>
    {
        public DataMapperTmdbConfig()
        {
        }

        #region IDataMapper<SearchMovie, TmdbServiceConfigurationInfo> Implementation

        public TmdbServiceConfigurationInfo Transform(TMDbConfig dataObject)
        {
            var images = dataObject.Images;
            return new TmdbServiceConfigurationInfo(
                baseUrl: images?.BaseUrl ?? string.Empty,
                secureBaseUrl: images?.SecureBaseUrl ?? string.Empty,
                backdropSizes: images?.BackdropSizes ?? [],
                posterSizes: images?.PosterSizes ?? []
            );
        }

        #endregion
    }
}
