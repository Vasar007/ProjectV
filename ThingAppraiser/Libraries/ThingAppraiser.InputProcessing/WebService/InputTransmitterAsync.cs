using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input.WebService
{
    public sealed class InputTransmitterAsync : IInputterAsync, IInputterBase, ITagable
    {
        private readonly IReadOnlyList<string> _thingNames;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(InputTransmitterAsync);

        #endregion

        public string StorageName { get; private set; } = string.Empty;


        public InputTransmitterAsync(IReadOnlyList<string> thingNames)
        {
            _thingNames = thingNames.ThrowIfNull(nameof(thingNames));
        }

        #region IInputterAsync Implementation

        public async Task ReadThingNames(ITargetBlock<string> queueToWrite, string storageName)
        {
            StorageName = storageName;

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            foreach (string thingName in _thingNames)
            {
                if (result.Add(thingName))
                {
                    await queueToWrite.SendAsync(thingName);
                }
            }
        }

        #endregion
    }
}
