using System.Linq;
using ProjectV.SteamService.Models;
using SteamWebApiLib.Models.BriefInfo;

namespace ProjectV.SteamService.Mappers
{
    internal sealed class DataMapperSteamContainer :
        IDataMapper<SteamAppBriefInfoList, SteamBriefInfoContainer>
    {
        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SteamAppBriefInfo, SteamBriefInfo> _mapperSteamBriefInfo =
            new DataMapperSteamBriefInfo();


        public DataMapperSteamContainer()
        {
        }

        #region IDataMapper<SteamAppBriefInfoList, SteamAppContainer> Implementation

        public SteamBriefInfoContainer Transform(SteamAppBriefInfoList dataObject)
        {
            var results = dataObject.Apps
                .Select(app => _mapperSteamBriefInfo.Transform(app))
                .ToList();

            return new SteamBriefInfoContainer(results);
        }

        #endregion
    }
}
