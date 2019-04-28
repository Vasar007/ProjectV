using System.Data;

namespace ThingAppraiser.DAL.Mappers
{
    /// <summary>
    /// Used to read current row from SQL data reader to DTO object.
    /// </summary>
    public interface IMapper<out T>
    {
        T ReadItem(IDataReader reader);
    }
}
