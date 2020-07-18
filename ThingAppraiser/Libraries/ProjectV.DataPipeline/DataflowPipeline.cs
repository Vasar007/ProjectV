using System.Threading.Tasks;
using Acolyte.Assertions;

namespace ProjectV.DataPipeline
{
    public sealed class DataflowPipeline
    {
        public InputtersFlow InputtersFlow { get; }

        public OutputtersFlow OutputtersFlow { get; }

        // Pipeline does not store neither crawlers nor appraisers flows because they are
        // implementation details.

        public DataflowPipeline(InputtersFlow inputtersFlow, OutputtersFlow outputtersFlow)
        {
            InputtersFlow = inputtersFlow.ThrowIfNull(nameof(inputtersFlow));
            OutputtersFlow = outputtersFlow.ThrowIfNull(nameof(outputtersFlow));
        }

        public async Task Execute(string input)
        {
            await InputtersFlow.ProcessAsync(new[] { input });
            await OutputtersFlow.CompletionTask;
        }
    }
}
