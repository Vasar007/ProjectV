using System.Data;

namespace ProjectV.DataAccessLayer.Mappers
{
    /// <summary>
    /// Used to read current row from SQL data reader to DTO object.
    /// </summary>
    /// <typeparam name="T">The type to convert data for mapper.</typeparam>
    public interface IMapper<T>
    {
        /// <summary>
        /// Reads data from reader and converts to specified type.
        /// </summary>
        /// <param name="reader">Instance which provides data.</param>
        /// <returns>Object which was filled by data reader.</returns>
        T ReadItem(IDataReader reader);
    }
}
