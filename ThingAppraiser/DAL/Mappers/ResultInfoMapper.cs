using System;
using System.Data;
using ThingAppraiser.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public class CResultInfoMapper : IMapper<CResultInfo>
    {
        public CResultInfoMapper()
        {
        }

        #region IMapper<CResultInfo> Implementation

        public CResultInfo ReadItem(IDataReader reader)
        {
            var item = new CResultInfo(
                (Int32)  reader["thing_id"],
                (Double) reader["rating_value"],
                (Guid)   reader["rating_id"]
            );
            return item;
        }

        #endregion
    }
}
