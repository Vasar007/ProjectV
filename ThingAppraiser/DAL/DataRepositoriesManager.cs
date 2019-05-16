using System;
using System.Collections.Generic;
using ThingAppraiser.Communication;
using ThingAppraiser.DAL.Repositories;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DAL
{
    public sealed class DataRepositoriesManager : IManager<IDataRepository>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<DataBaseManager>();

        private readonly Dictionary<Type, IDataRepository> _repositories =
            new Dictionary<Type, IDataRepository>();


        public DataRepositoriesManager()
        {
        }

        private static MinMaxDenominator GetAdditionalInfoDouble(IDataRepository repository,
            string infoToRequest)
        {
            var (min, max) = repository.GetMinMax<double>(infoToRequest);
            return new MinMaxDenominator(min, max);
        }

        private static MinMaxDenominator GetAdditionalInfoInt32(IDataRepository repository,
            string infoToRequest)
        {
            var (min, max) = repository.GetMinMax<int>(infoToRequest);
            return new MinMaxDenominator(min, max);
        }

        #region IManager<IRepository> Implementation

        public void Add(IDataRepository item)
        {
            item.ThrowIfNull(nameof(item));

            if (!_repositories.ContainsKey(item.TypeId))
            {
                _repositories.Add(item.TypeId, item);
            }
        }

        public bool Remove(IDataRepository item)
        {
            item.ThrowIfNull(nameof(item));
            return _repositories.Remove(item.TypeId);
        }

        #endregion

        public List<List<BasicInfo>> GetResultsFromDb()
        {
            var results = new List<List<BasicInfo>>();
            foreach (IDataRepository repository in _repositories.Values)
            {
                results.Add(repository.GetAllData());
            }
            return results;
        }

        public List<RawDataContainer> GetResultsFromDbWithAdditionalInfo()
        {
            var results = new List<RawDataContainer>();
            foreach (IDataRepository repository in _repositories.Values)
            {
                List<BasicInfo> data = repository.GetAllData();
                results.Add(AddAdditionalParameters(data));
            }
            return results;
        }

        public BasicInfo GetProperDataHandlerById(int thingId, Type dataHandlerType)
        {
            if (!_repositories.TryGetValue(dataHandlerType, out IDataRepository repository))
            {
                var ex = new ArgumentException(
                    $"Type {dataHandlerType} didn't exist in repositories!", nameof(dataHandlerType)
                );
                _logger.Error(ex, "Tried to get invalid thing data handler.");
                throw ex;
            }
            return repository.GetItemById(thingId);
        }

        public void PutResultsToDb(List<List<BasicInfo>> results)
        {
            foreach (List<BasicInfo> datum in results)
            {
                // Skip empty collections of data.
                if (datum.IsNullOrEmpty()) continue;

                // Suggest that all types in collection are identical.
                if (!_repositories.TryGetValue(datum[0].GetType(), out IDataRepository repository))
                {
                    _logger.Info($"Type {datum[0].GetType()} didn't save!");
                    GlobalMessageHandler.OutputMessage(
                        $"Type {datum[0].GetType()} didn't save!"
                    );
                    continue;
                }
                foreach (BasicInfo info in datum)
                {
                    if (repository.Contains(info.ThingId))
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

        private RawDataContainer AddAdditionalParameters(List<BasicInfo> data)
        {
            var container = new RawDataContainer(data);
            switch (data[0])
            {
                case TmdbMovieInfo _:
                {
                    if (_repositories.TryGetValue(typeof(TmdbMovieInfo),
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

                case BasicInfo _:
                {
                    if (_repositories.TryGetValue(typeof(BasicInfo), 
                                                  out IDataRepository repository))
                    {
                        container.AddParameter(
                            "VoteCount", GetAdditionalInfoInt32(repository, "vote_count")
                        );
                        container.AddParameter(
                            "VoteAverage", GetAdditionalInfoDouble(repository, "vote_average")
                        );
                    }
                    break;
                }

                default:
                    _logger.Warn($"Unregornized data type: {data[0].GetType().FullName}");
                    break;
            }
            return container;
        }
    }
}
