using System.Collections.Generic;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.Tests.Scenarios.Helpers;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Polling
{
    /// <summary>
    /// Per-family base class for Telegram polling scenario tests against
    /// <c>ProjectV.TelegramBotWebService</c>. Sibling to
    /// <c>TelegramWebhookScenarioBaseTest</c> — all shared host wiring (the
    /// bot-service / comm-client DI swaps, the NLog capture hook, the dummy
    /// <c>BotToken</c>) lives in <see cref="TelegramScenarioBaseTest" />;
    /// this class contributes only the polling working mode.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The polling path differs from the webhook path in two ways. First, the
    /// production <c>PoolingProcessor</c> is a <c>BackgroundService</c> the
    /// host starts when the working mode is
    /// <see cref="TelegramBotWebServiceWorkingMode.PollingViaHostedService" />.
    /// The processor calls <c>IBotPolling.StartReceivingUpdatesAsync</c>, which
    /// in turn calls <c>IBotService.DeleteWebhookAsync</c> and then
    /// <c>IBotService.BotClient.ReceiveAsync(...)</c> — the production polling
    /// loop. Second, polling tests assert on the in-process call-count recorded
    /// by the <c>StubBotService</c> (one <c>SendMessageAsync</c> call per
    /// update the production handler chain drains) rather than on an HTTP
    /// response, because the polling path has no outbound HTTP response
    /// surface.
    /// </para>
    /// <para>
    /// The <c>PoolingProcessor</c> factory (<c>PoolingProcessor.Create</c>)
    /// resolves <c>IBotPolling</c> from the container at host start;
    /// <c>BotPolling</c>'s ctor pulls <c>IBotService</c>, which by then is the
    /// test-side stub (the override registered in <c>ConfigureTestServices</c>
    /// runs AFTER <c>Startup.ConfigureServices</c> but BEFORE the host starts
    /// its <c>IHostedService</c> instances, so the substitution wins).
    /// Concrete scenarios pass a pre-configured <c>StubTelegramBotClient</c>
    /// directly to the constructor to supply the update sequence.
    /// </para>
    /// </remarks>
    public abstract class TelegramPollingScenarioBaseTest : TelegramScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramPollingScenarioBaseTest" /> class.
        /// </summary>
        /// <param name="botClientStub">
        /// Optional pre-built <see cref="ITelegramBotClient" /> stub. When
        /// <c>null</c>, a bare <c>StubTelegramBotClient</c> (no updates
        /// configured) is used — the polling loop will fetch an empty
        /// batch on every call and keep looping until cancellation, which is
        /// useful only for tests that do not assert on update consumption.
        /// </param>
        /// <param name="extraConfiguration">
        /// Optional in-memory configuration overrides layered on top of the
        /// host's <c>appsettings.json</c>. The base class always layers a
        /// <c>WorkingMode=PollingViaHostedService</c> override plus a dummy
        /// <c>BotToken</c> override.
        /// </param>
        protected TelegramPollingScenarioBaseTest(
            ITelegramBotClient? botClientStub = null,
            IReadOnlyDictionary<string, string?>? extraConfiguration = null)
            : base(
                workingMode: TelegramBotWebServiceWorkingMode.PollingViaHostedService,
                botClientStub: botClientStub,
                extraConfiguration: extraConfiguration)
        {
        }
    }
}
