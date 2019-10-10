using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.DataPipeline;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    public sealed class AppraisersManagerAsync : IManager<IAppraiserAsync>, IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManagerAsync>();

        private readonly Dictionary<Type, IList<IAppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, IList<IAppraiserAsync>>();

        private readonly bool _outputResults;

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private bool _disposed;


        public AppraisersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<AppraiserAsync> Implementation

        public void Add(IAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersAsync.TryGetValue(item.TypeId, out IList<IAppraiserAsync> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersAsync.Add(item.TypeId, new List<IAppraiserAsync> { item });
            }
        }

        public bool Remove(IAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _appraisersAsync.Remove(item.TypeId);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cancellationTokenSource.Dispose();
        }

        #endregion

        public AppraisersFlow GetAllRatings()
        {
            var appraisersFunc = new List<Appraiser>();
            foreach ((Type type, IList<IAppraiserAsync> appraisersAsync) in _appraisersAsync)
            {
                foreach (var appraiserAsync in appraisersAsync)
                {
                    var appraiser = new Appraiser(entityInfo => appraiserAsync.GetRatings(entityInfo, _outputResults), type);
                    appraisersFunc.Add(appraiser);
                }
            }

            var appraisersFlow = new AppraisersFlow(appraisersFunc);

            _logger.Info("Constructed appraisers pipeline.");
            return appraisersFlow;
        }
    }
}
