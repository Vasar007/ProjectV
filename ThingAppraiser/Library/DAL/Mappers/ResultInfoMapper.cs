using System;
using System.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DAL.Mappers
{
    public class ResultInfoMapper : IMapper<ResultInfo>
    {
        public ResultInfoMapper()
        {
        }

        #region IMapper<ResultInfo> Implementation

        public ResultInfo ReadItem(IDataReader reader)
        {
            var item = new ResultInfo(
                (int)    reader["thing_id"],
                (double) reader["rating_value"],
                (Guid)   reader["rating_id"]
            );
            return item;
        }

        #endregion
    }
}
