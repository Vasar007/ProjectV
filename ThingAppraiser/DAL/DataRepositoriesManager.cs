using System;
using System.Collections.Generic;
using ThingAppraiser.Communication;
using ThingAppraiser.DAL.Repositories;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DAL
{
    public sealed class CDataRepositoriesManager : IManager<IDataRepository>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CDataBaseManager>();

        private readonly Dictionary<Type, IDataRepository> _repositories =
            new Dictionary<Type, IDataRepository>();


        public CDataRepositoriesManager()
        {
        }

        private static CMinMaxDenominator GetAdditionalInfoDouble(IDataRepository repository,
            String infoToRequest)
        {
            var (min, max) = repository.GetMinMax<Double>(infoToRequest);
            return new CMinMaxDenominator(min, max);
        }

        private static CMinMaxDenominator GetAdditionalInfoInt32(IDataRepository repository,
            String infoToRequest)
        {
            var (min, max) = repository.GetMinMax<Int32>(infoToRequest);
            return new CMinMaxDenominator(min, max);
        }

        #region IManager<IRepository> Implementation

        public void Add(IDataRepository item)
        {
            item.ThrowIfNull(nameof(item));

            if (!_repositories.ContainsKey(item.TypeID))
            {
                _repositories.Add(item.TypeID, item);
            }
        }

        public Boolean Remove(IDataRepository item)
        {
            item.ThrowIfNull(nameof(item));
            return _repositories.Remove(item.TypeID);
        }

        #endregion

        public List<List<CBasicInfo>> GetResultsFromDB()
        {
            var results = new List<List<CBasicInfo>>();
            foreach (IDataRepository repository in _repositories.Values)
            {
                results.Add(repository.GetAllData());
            }
            return results;
        }

        public List<CRawDataContainer> GetResultsFromDBWithAdditionalInfo()
        {
            var results = new List<CRawDataContainer>();
            foreach (IDataRepository repository in _repositories.Values)
            {
                List<CBasicInfo> data = repository.GetAllData();
                results.Add(AddAdditionalParameters(data));
            }
            return results;
        }

        public CBasicInfo GetProperDataHandlerByID(Int32 thingID, Type dataHandlerType)
        {
            if (!_repositories.TryGetValue(dataHandlerType, out IDataRepository repository))
            {
                var ex = new ArgumentException(
                    $"Type {dataHandlerType} didn't exist in repositories!", nameof(dataHandlerType)
                );
                s_logger.Error(ex, "Tried to get invalid thing data handler.");
                throw ex;
            }
            return repository.GetItemByID(thingID);
        }

        public void PutResultsToDB(List<List<CBasicInfo>> results)
        {
            foreach (List<CBasicInfo> datum in results)
            {
                // Skip empty collections of data.
                if (datum.IsNullOrEmpty()) continue;

                // Suggest that all types in collection are identical.
                if (!_repositories.TryGetValue(datum[0].GetType(), out IDataRepository repository))
                {
                    s_logger.Info($"Type {datum[0].GetType()} didn't save!");
                    SGlobalMessageHandler.OutputMessage(
                        $"Type {datum[0].GetType()} didn't save!"
                    );
                    continue;
                }
                foreach (CBasicInfo info in datum)
                {
                    if (repository.Contains(info.ThingID))
                    {
                        repository.UpdateItem(info);
                    }
                    else
                    {
                        repository.InsertItem(info);
                    }
                }
            }
        }

        public void DeleteData()
        {
            foreach (IDataRepository repository in _repositories.Values)
            {
                repository.DeleteAllData();
            }
        }

        private CRawDataContainer AddAdditionalParameters(List<CBasicInfo> data)
        {
            var container = new CRawDataContainer(data);
            switch (data[0])
            {
                case CMovieTMDBInfo _:
                {
                    if (_repositories.TryGetValue(typeof(CMovieTMDBInfo),
                                                  out IDataRepository repository))
                    {
                        container.AddParameter(
                            "VoteCount", GetAdditionalInfoInt32(repository, "vote_count")
                        );
                        container.AddParameter(
                            "VoteAverage", GetAdditionalInfoDouble(repository, "vote_average")
                        );
                        container.AddParameter(
                            "Popularity", GetAdditionalInfoDouble(repository, "popularity")
                        );
                    }
                    break;
                }

                case CBasicInfo _:
                {
                    if (_repositories.TryGetValue(typeof(CBasicInfo), 
                                                  out IDataRepository repository))
                    {
                        container.AddParameter(
                            "VoteCount", GetAdditionalInfoDouble(repository, "vote_count")
                        );
                        container.AddParameter(
                            "VoteAverage", GetAdditionalInfoDouble(repository, "vote_average")
                        );
                    }
                    break;
                }
            }
            return container;
        }
    }
}
