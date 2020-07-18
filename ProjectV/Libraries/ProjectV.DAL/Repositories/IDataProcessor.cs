namespace ProjectV.DAL.Repositories
{
    public interface IDataProcessor
    {
        T GetMinimum<T>(string columnName, string tableName);

        T GetMaximum<T>(string columnName, string tableName);

        (T, T) GetMinMax<T>(string columnName, string tableName);
    }
}
