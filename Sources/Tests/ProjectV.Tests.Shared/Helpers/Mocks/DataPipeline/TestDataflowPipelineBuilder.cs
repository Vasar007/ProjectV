using Acolyte.Assertions;
using ProjectV.DataPipeline;

namespace ProjectV.Tests.Shared.Helpers.Mocks.DataPipeline
{
    /// <summary>
    /// Builder for real <see cref="DataflowPipeline" /> instances populated
    /// with caller-supplied <see cref="DataPipeline.InputtersFlow" /> +
    /// <see cref="DataPipeline.OutputtersFlow" /> stages (Decision D-33
    /// fallback). <see cref="DataflowPipeline" /> is a <c>sealed</c> class
    /// with no substitution-friendly interface seam — its constructor takes
    /// real flow instances and exposes them as read-only properties, so this
    /// builder returns a real pipeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Wiring an entirely-empty pipeline (no inputters, no outputters) is
    /// supported for shape/property tests, but exercising
    /// <see cref="DataflowPipeline.Execute(string)" /> end-to-end requires
    /// fully-composed flows because Gridsum.DataflowEx blocks complete only
    /// when every upstream dependency has signalled completion (see
    /// <c>02-05-SUMMARY.md</c> § "Deviations" — the empty-pipeline hang).
    /// </para>
    /// <para>
    /// Crawlers / Appraisers flows are intentionally NOT carried as
    /// properties on <see cref="DataflowPipeline" />; they are implementation
    /// details of <see cref="DataflowPipeline.Execute(string)" /> in the
    /// production path (here, the test caller wires its own composition
    /// outside the pipeline when needed for integration coverage).
    /// </para>
    /// </remarks>
    public sealed class TestDataflowPipelineBuilder
    {
        private InputtersFlow? _inputtersFlow;
        private OutputtersFlow? _outputtersFlow;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestDataflowPipelineBuilder" /> class. No flow stages
        /// are configured until one of the <c>With*</c> methods is called.
        /// </summary>
        public TestDataflowPipelineBuilder()
        {
        }

        /// <summary>
        /// Convenience factory returning a <see cref="DataflowPipeline" />
        /// composed of empty <see cref="DataPipeline.InputtersFlow" /> +
        /// <see cref="DataPipeline.OutputtersFlow" /> stages — useful for
        /// constructor / property tests that do not exercise
        /// <see cref="DataflowPipeline.Execute(string)" />.
        /// </summary>
        public static DataflowPipeline CreateWithoutSetup()
        {
            return new TestDataflowPipelineBuilder().Build();
        }

        /// <summary>
        /// Overrides the <see cref="DataPipeline.InputtersFlow" /> stage. If
        /// unset, an empty stage is constructed by <see cref="Build" />.
        /// </summary>
        /// <param name="inputtersFlow">Flow instance. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestDataflowPipelineBuilder WithInputtersFlow(InputtersFlow inputtersFlow)
        {
            inputtersFlow.ThrowIfNull(nameof(inputtersFlow));

            _inputtersFlow = inputtersFlow;
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="DataPipeline.OutputtersFlow" /> stage. If
        /// unset, an empty stage is constructed by <see cref="Build" />.
        /// </summary>
        /// <param name="outputtersFlow">Flow instance. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestDataflowPipelineBuilder WithOutputtersFlow(OutputtersFlow outputtersFlow)
        {
            outputtersFlow.ThrowIfNull(nameof(outputtersFlow));

            _outputtersFlow = outputtersFlow;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="DataflowPipeline" /> instance with the
        /// configured (or defaulted-empty) stages.
        /// </summary>
        public DataflowPipeline Build()
        {
            InputtersFlow inputtersFlow = _inputtersFlow
                ?? new InputtersFlow(Array.Empty<Func<string, IEnumerable<string>>>());
            OutputtersFlow outputtersFlow = _outputtersFlow
                ?? new OutputtersFlow(Array.Empty<Action<Models.Internal.RatingDataContainer>>());

            return new DataflowPipeline(inputtersFlow, outputtersFlow);
        }
    }
}
