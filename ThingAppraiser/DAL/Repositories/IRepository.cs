using System;
using System.Collections.Generic;

namespace ThingAppraiser.DAL.Repositories
{
    public interface IRepository<TData, in TIdentifier> : ITagable, ITypeID
    {
        Boolean Contains(TIdentifier id);

        void InsertItem(TData item);

        TData GetItemByID(TIdentifier id);

        List<TData> GetAllData();

        void UpdateItem(TData item);

        void DeleteItemByID(TIdentifier id);

        void DeleteAllData();
    }
}
