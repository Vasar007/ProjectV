using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input.File
{
    public interface IFileReaderAsync
    {
        Task ReadFile(ITargetBlock<string> queue, string filename);

        Task ReadCsvFile(ITargetBlock<string> queue, string filename);
    }
}
