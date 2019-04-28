using System;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IDataProcessor
    {
        T GetMinimum<T>(String columnName, String tableName);

        T GetMaximum<T>(String columnName, String tableName);

        (T, T) GetMinMax<T>(String columnName, String tableName);
    }
}
