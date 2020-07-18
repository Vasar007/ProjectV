using SteamWebApiLib.Models.BriefInfo;
using ProjectV.SteamService.Models;

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
                name:  dataObject.Name
            );
        }

        #endregion
    }
}
