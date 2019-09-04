using System;
using System.Collections.Generic;
using System.Linq;
using SteamWebApiLib.Models.AppDetails;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.SteamService.Mappers
{
    public sealed class DataMapperSteamGame : IDataMapper<SteamApp, SteamGameInfo>
    {
        public DataMapperSteamGame()
        {
        }

        #region IDataMapper<SteamApp, SteamGameInfo> Implementation

        public SteamGameInfo Transform(SteamApp dataObject)
        {
            DateTime releaseDate = DateTime.Parse(dataObject.ReleaseDate.Date);
            decimal price = Convert.ToDecimal(dataObject.PriceOverview.Final);
            IReadOnlyList<int> genreIds = dataObject.Genres.Select(genre => genre.Id).ToList();

            return new SteamGameInfo(
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
        }

        #endregion
    }
}
