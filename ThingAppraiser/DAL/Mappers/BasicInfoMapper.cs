using System;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class CBasicInfoMapper : IMapper<CBasicInfo>
    {
        public CBasicInfoMapper()
        {
        }

        #region IMapper<CBasicInfo> Implementation

        public CBasicInfo ReadItem(IDataReader reader)
        {
            var item = new CBasicInfo(
                id:           (Int32)  reader["thing_id"],
                title:        (String) reader["title"],
                vote_count:   (Int32)  reader["vote_count"],
                vote_average: (Double) reader["vote_average"]
            );
            return item;
        }

        #endregion
    }
}
