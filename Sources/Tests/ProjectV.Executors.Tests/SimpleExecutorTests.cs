using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    /// <c>Docs/Testing/Coverage/test-coverage.md</c>: the test asserts the CURRENT
    /// (anti-pattern) behaviour — the parameterless overload is an unfinished stub
    /// that throws <see cref="NotImplementedException" /> rather than executing real
    /// job logic. The eventual fix that wires the executor to the persisted job
    /// config is deferred to a future phase. When that fix lands, this test should
    /// be replaced with one that exercises the real persisted execution path.
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
        public async Task ExecuteAsync_Parameterless_ThrowsNotImplementedException()
        {
            // Arrange.
            JobInfo jobInfo = CreateJobInfo();
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
                         "the parameterless overload is an anti-pattern stub whose current " +
                         "behaviour is a synchronous throw with the in-code TODO message"
                     );
        }

        [Fact]
        public void Constructor_OnAllArgumentsProvided_CreatesInstance()
        {
            // Arrange.
            JobInfo jobInfo = CreateJobInfo();

            // Act.
            var act = () => CreateSimpleExecutorWithAllArguments(jobInfo);

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenNullValueProvided()
        {
            // Arrange.
            var actions = new List<(Action act, string paramName)>
            {
                (() => CreateSimpleExecutorWithAllArguments(null!), "jobInfo"),
            };

            // Act. / Assert.
            foreach ((Action act, string paramName) in actions)
            {
                act.Should()
                   .Throw<ArgumentNullException>()
                   .WithParameterName(paramName);
            }
        }

        [Fact]
        public void Constructor_WithZeroExecutionsNumber_ThrowsArgumentOutOfRangeException()
        {
            // Arrange.
            JobInfo jobInfo = CreateJobInfo();

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
            JobInfo jobInfo = CreateJobInfo();
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

        #region Helper Methods

        /// <summary>
        /// Creates a <see cref="SimpleExecutor" /> from its constructor
        /// dependencies (only <c>jobInfo</c> is a null-guarded reference
        /// dependency; the numeric arguments use fixed valid values).
        /// Constructor tests pass <c>jobInfo</c> as <c>null!</c> to exercise
        /// the null-guard.
        /// </summary>
        private static SimpleExecutor CreateSimpleExecutorWithAllArguments(
            JobInfo jobInfo)
        {
            return new SimpleExecutor(
                jobInfo: jobInfo,
                executionsNumber: 1,
                delayTime: TimeSpan.Zero
            );
        }

        private static JobInfo CreateJobInfo()
        {
            return JobInfo.Create(
                name: "ProjectV.Executors.Tests.SimpleExecutorTests",
                config: "<config />"
            );
        }

        #endregion
    }
}
