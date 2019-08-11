using System;
using System.Linq;
using SteamWebApiLib.Models.AppDetails;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers.Steam
{
    public class DataMapperSteamGame : IDataMapper<SteamApp, SteamGameInfo>
    {
        public DataMapperSteamGame()
        {
        }

        #region IDataMapper<SteamApp, SteamGameInfo> Implementation

        public SteamGameInfo Transform(SteamApp dataObject)
        {
            var releaseDate = DateTime.Parse(dataObject.ReleaseDate.Date);
            var price = Convert.ToDecimal(dataObject.PriceOverview.Final);
            var genreIds = dataObject.Genres.Select(genre => genre.Id).ToList();

            var result = new SteamGameInfo(
                thingId:     dataObject.SteamAppId,
                title:       dataObject.Name,
                voteCount:   dataObject.PriceOverview.DiscountPercent,
                voteAverage: dataObject.PriceOverview.Initial,
                overview:    dataObject.ShortDescription,
                releaseDate: releaseDate,
                price:       price,
                requiredAge: dataObject.RequiredAge,
                genreIds:    genreIds,
                posterPath:  dataObject.HeaderImage
            );
            return result;
        }

        #endregion
    }
}
