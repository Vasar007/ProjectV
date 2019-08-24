using System.Data;

namespace ThingAppraiser.DAL.Mappers
{
    public sealed class TwoValuesMapper<T> : IMapper<(T, T)>
    {
        public TwoValuesMapper()
        {
        }

        #region IMapper<(T, T)> Implementation

        public (T, T) ReadItem(IDataReader reader)
        {
            return ((T) reader.GetValue(0), (T) reader.GetValue(1));
        }

        #endregion
    }
}
