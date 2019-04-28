using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.IO.Input
{
    public interface IFileReaderAsync
    {
        Task ReadFile(BufferBlock<String> queue, String filename);

        Task ReadCsvFile(BufferBlock<String> queue, String filename);
    }
}
