using ThingAppraiser.Data.Crawlers;
using TMDbLib.Objects.General;

namespace ThingAppraiser.Crawlers.Mappers
{
    public class DataMapperTmdbConfig : IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo>
    {
        public DataMapperTmdbConfig()
        {
        }

        #region IDataMapper<SearchMovie, MovieInfo> Implementation

        public TmdbServiceConfigurationInfo Transform(TMDbConfig dataObject)
        {
            var result = new TmdbServiceConfigurationInfo(
                baseUrl:       dataObject.Images.BaseUrl,
                secureBaseUrl: dataObject.Images.SecureBaseUrl,
                backdropSizes: dataObject.Images.BackdropSizes,
                posterSizes:   dataObject.Images.PosterSizes
            );
            return result;
        }

        #endregion
    }
}
