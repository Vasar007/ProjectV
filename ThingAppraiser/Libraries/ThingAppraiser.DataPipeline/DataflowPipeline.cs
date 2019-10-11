using System.Threading.Tasks;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DataPipeline
{
    public sealed class DataflowPipeline
    {
        public InputtersFlow InputtersFlow { get; }

        public OutputtersFlow OutputtersFlow { get; }


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
