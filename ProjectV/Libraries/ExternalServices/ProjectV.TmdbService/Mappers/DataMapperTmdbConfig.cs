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
            return new TmdbServiceConfigurationInfo(
                baseUrl:       dataObject.Images.BaseUrl,
                secureBaseUrl: dataObject.Images.SecureBaseUrl,
                backdropSizes: dataObject.Images.BackdropSizes,
                posterSizes:   dataObject.Images.PosterSizes
            );
        }

        #endregion
    }
}
