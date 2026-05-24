using Microsoft.Extensions.Time.Testing;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Thin wrapper around <see cref="FakeTimeProvider" /> for tests that
    /// need to control "now" and advance a virtual clock. Bridges the
    /// project's preferred <see cref="TimeProvider" /> abstraction with the
    /// xUnit test harness, giving tests deterministic control over time.
    /// </summary>
    public static class TestTimeHelper
    {
        /// <summary>
        /// Creates a <see cref="FakeTimeProvider" /> initialized at the
        /// supplied <paramref name="initialNow" /> instant.
        /// </summary>
        /// <param name="initialNow">Initial value for "now".</param>
        /// <returns>A fresh <see cref="FakeTimeProvider" />.</returns>
        public static FakeTimeProvider Create(DateTimeOffset initialNow)
        {
            return new FakeTimeProvider(initialNow);
        }

        /// <summary>
        /// Advances the supplied <paramref name="timeProvider" /> by the
        /// requested <paramref name="delta" />. Pure convenience wrapper
        /// around <see cref="FakeTimeProvider.Advance(TimeSpan)" /> so test
        /// bodies stay readable.
        /// </summary>
        /// <param name="timeProvider">Fake time provider to advance.</param>
        /// <param name="delta">Amount of time to advance by.</param>
        public static void Advance(FakeTimeProvider timeProvider, TimeSpan delta)
        {
            ArgumentNullException.ThrowIfNull(timeProvider);

            timeProvider.Advance(delta);
        }
    }
}
