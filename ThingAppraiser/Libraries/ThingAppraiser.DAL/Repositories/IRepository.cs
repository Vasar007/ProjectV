using System.Collections.Generic;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IRepository<TData, in TIdentifier> : IRepositoryBase, ITagable, ITypeId
    {
        bool Contains(TIdentifier id);

        void InsertItem(TData item);

        TData GetItemById(TIdentifier id);

        IReadOnlyList<TData> GetAllData();

        void UpdateItem(TData item);

        void DeleteItemById(TIdentifier id);

        void DeleteAllData();
    }
}
