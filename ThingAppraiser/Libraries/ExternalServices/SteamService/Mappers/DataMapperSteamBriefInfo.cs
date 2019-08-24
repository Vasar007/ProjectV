using SteamWebApiLib.Models.BriefInfo;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.SteamService.Mappers
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
