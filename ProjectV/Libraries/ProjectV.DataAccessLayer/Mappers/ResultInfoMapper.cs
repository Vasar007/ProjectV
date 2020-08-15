using System;
using System.Data;
using ProjectV.Models.Internal;

namespace ProjectV.DataAccessLayer.Mappers
{
    public sealed class ResultInfoMapper : IMapper<ResultInfo>
    {
        public ResultInfoMapper()
        {
        }

        #region IMapper<ResultInfo> Implementation

        public ResultInfo ReadItem(IDataReader reader)
        {
            return new ResultInfo(
                (int)    reader["thing_id"],
                (double) reader["rating_value"],
                (Guid)   reader["rating_id"]
            );
        }

        #endregion
    }
}
