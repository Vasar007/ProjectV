using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public sealed class CInputManagerAsync : IManager<IInputterAsync>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CInputManagerAsync>();

        private readonly String _defaultFilename;

        private readonly List<IInputterAsync> _inputtersAsync = new List<IInputterAsync>();


        public CInputManagerAsync(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));
        }

        #region IManager<IInputterAsync> Implementation

        public void Add(IInputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_inputtersAsync.Contains(item))
            {
                _inputtersAsync.Add(item);
            }
        }

        public Boolean Remove(IInputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _inputtersAsync.Remove(item);
        }

        #endregion

        public async Task<Boolean> GetNames(BufferBlock<String> queueToWrite, String storageName)
        {
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            List<Task<Boolean>> producers = _inputtersAsync.Select(
                inputterAsync => TryReadThingNames(inputterAsync, queueToWrite, storageName)
            ).ToList();

            Boolean[] statuses = await Task.WhenAll(producers);
            queueToWrite.Complete();

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info($"{statuses.Length} Things were found.");
                return true;
            }

            s_logger.Info($"No Things were found in \"{storageName}\".");
            return false;
        }

        private static async Task<Boolean> TryReadThingNames(IInputterAsync inputterAsync,
            BufferBlock<String> queueToWrite, String storageName)
        {
            try
            {
                await inputterAsync.ReadThingNames(queueToWrite, storageName);
            }
            catch (Exception ex)
            {
                s_logger.Warn(ex, "Couldn't get access to the storage.");
                return false;
            }
            return true;
        }
    }
}
