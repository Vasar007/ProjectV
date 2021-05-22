using ProjectV.SteamService.Models;
using SteamWebApiLib.Models.BriefInfo;

namespace ProjectV.SteamService.Mappers
{
    internal sealed class DataMapperSteamBriefInfo :
        IDataMapper<SteamAppBriefInfo, SteamBriefInfo>
    {
        public DataMapperSteamBriefInfo()
        {
        }

        #region IDataMapper<SteamAppBriefInfo, SteamBriefInfo> Implementation

        public SteamBriefInfo Transform(SteamAppBriefInfo dataObject)
        {
            return new SteamBriefInfo(
                appId: dataObject.AppId,
                name: dataObject.Name
            );
        }

        #endregion
    }
}
