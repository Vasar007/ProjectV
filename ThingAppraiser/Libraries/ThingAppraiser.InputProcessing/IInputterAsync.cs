using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input
{
    public interface IInputterAsync : IInputterBase, ITagable
    {
        Task ReadThingNames(ITargetBlock<string> queueToWrite, string storageName);
    }
}
