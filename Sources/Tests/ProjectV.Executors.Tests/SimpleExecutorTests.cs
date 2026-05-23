using System;
using AwesomeAssertions;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Tests.Shared.ForTests;
using Xunit;

namespace ProjectV.Executors.Tests
{
    /// <summary>
    /// Unit tests for <see cref="SimpleExecutor" />, focused on the current
    /// parameterless <see cref="SimpleExecutor.ExecuteAsync()" /> contract:
    /// the overload throws <see cref="NotImplementedException" /> synchronously
    /// because the in-code <c>TODO</c> ("Take config from DB. / Create Shell.
    /// / Execute Shell with data.") has not been implemented yet.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This row is documented as <c>tested around</c> in
    /// <c>Docs/Testing/Coverage/test-coverage.md</c> per
    /// <c>ARCHITECTURE.md</c> § "Anti-Patterns": the test asserts the CURRENT
    /// (anti-pattern) behaviour — the eventual fix that wires the executor to
    /// the persisted job config is deferred to a future phase per
    /// <c>02-CONTEXT.md</c> § "Deferred Ideas". When that fix lands, this
    /// test should be replaced with one that exercises the real persisted
    /// execution path.
    /// </para>
    /// <para>
    /// The throw is synchronous (the production method is not <c>async</c>;
    /// it raises before returning any <see cref="System.Threading.Tasks.Task" />),
    /// but the method's signature is <c>Task&lt;IReadOnlyList&lt;ServiceStatus&gt;&gt;</c>
    /// so we use <c>ThrowAsync&lt;T&gt;</c> for AwesomeAssertions — it
    /// handles both sync throws inside a Task-returning method and async
    /// exceptions transparently.
    /// </para>
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class SimpleExecutorTests : BaseMockTest
    {
        public SimpleExecutorTests()
        {
        }

        [Fact]
        public async System.Threading.Tasks.Task ExecuteAsync_Parameterless_ThrowsNotImplementedException()
        {
            // Arrange.
            var jobInfo = JobInfo.Create(
                name: "ProjectV.Executors.Tests.SimpleExecutorTests",
                config: "<config />"
            );
            var sut = new SimpleExecutor(
                jobInfo: jobInfo,
                executionsNumber: 1,
                delayTime: TimeSpan.Zero
            );

            // Act.
            var act = () => sut.ExecuteAsync();

            // Assert.
            await act.Should()
                     .ThrowAsync<NotImplementedException>(
                         "the parameterless overload is documented as an anti-pattern stub " +
                         "in ARCHITECTURE.md and 02-CONTEXT.md § Deferred Ideas — its current " +
                         "behaviour is a synchronous throw with the in-code TODO message"
                     );
        }

        [Fact]
        public void Constructor_WithNullJobInfo_ThrowsArgumentNullException()
        {
            // Arrange. / Act.
            var act = () => new SimpleExecutor(
                jobInfo: null!,
                executionsNumber: 1,
                delayTime: TimeSpan.Zero
            );

            // Assert.
            act.Should()
               .Throw<ArgumentNullException>()
               .WithParameterName("jobInfo");
        }

        [Fact]
        public void Constructor_WithZeroExecutionsNumber_ThrowsArgumentOutOfRangeException()
        {
            // Arrange.
            var jobInfo = JobInfo.Create(
                name: "ProjectV.Executors.Tests.SimpleExecutorTests",
                config: "<config />"
            );

            // Act.
            var act = () => new SimpleExecutor(
                jobInfo: jobInfo,
                executionsNumber: 0,
                delayTime: TimeSpan.Zero
            );

            // Assert.
            act.Should()
               .Throw<ArgumentOutOfRangeException>()
               .WithParameterName("executionsNumber");
        }

        [Fact]
        public void Constructor_HappyPath_ExposesIdAndExecutionPropertiesFromArguments()
        {
            // Arrange.
            var jobInfo = JobInfo.Create(
                name: "ProjectV.Executors.Tests.SimpleExecutorTests",
                config: "<config />"
            );
            var delayTime = TimeSpan.FromMilliseconds(123);
            const int executionsNumber = 2;

            // Act.
            var sut = new SimpleExecutor(
                jobInfo: jobInfo,
                executionsNumber: executionsNumber,
                delayTime: delayTime
            );

            // Assert.
            sut.Id.Should().Be(jobInfo.Id);
            sut.ExecutionsNumber.Should().Be(executionsNumber);
            sut.DelayTime.Should().Be(delayTime);
            sut.RestartPoint.Should().Be(RestartPointKind.None);
        }
    }
}
