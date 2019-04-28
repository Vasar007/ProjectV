using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input
{
    public interface IInputterAsync : ITagable
    {
        Task ReadThingNames(BufferBlock<String> queueToWrite, String storageName);
    }
}
