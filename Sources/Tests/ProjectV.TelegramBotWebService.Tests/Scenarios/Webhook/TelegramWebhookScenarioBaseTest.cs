using System.Collections.Generic;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.Tests.Scenarios.Helpers;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Per-family base class for Telegram webhook scenario tests against
    /// <c>ProjectV.TelegramBotWebService</c>. All shared host wiring (the
    /// bot-service / comm-client DI swaps, the NLog capture hook, the dummy
    /// <c>BotToken</c>) lives in <see cref="TelegramScenarioBaseTest" />;
    /// this class contributes only the webhook working mode.
    /// </summary>
    /// <remarks>
    /// The working mode is
    /// <see cref="TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup" />
    /// so the host does NOT register the <c>PoolingProcessor</c> /
    /// <c>ConfigureWebhook</c> hosted services (both of which would resolve
    /// <c>IBotService</c> during host startup, before the DI override in
    /// <c>ConfigureTestServices</c> has a chance to win). Webhook scenarios
    /// then POST synthetic updates to the production endpoint and assert on
    /// the HTTP response plus the calls the handler chain recorded in
    /// <c>BotServiceStub.CalledMethodNames</c>.
    /// </remarks>
    public abstract class TelegramWebhookScenarioBaseTest : TelegramScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramWebhookScenarioBaseTest" /> class with default
        /// (bare, no-setup) bot-client stub and no extra configuration
        /// overrides.
        /// </summary>
        protected TelegramWebhookScenarioBaseTest()
            : this(botClientStub: null, extraConfiguration: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramWebhookScenarioBaseTest" /> class.
        /// </summary>
        /// <param name="botClientStub">
        /// Optional pre-built <see cref="ITelegramBotClient" /> stub. When
        /// <c>null</c>, a bare <c>StubTelegramBotClient</c> (no updates
        /// configured) is used.
        /// </param>
        /// <param name="extraConfiguration">
        /// Optional in-memory configuration overrides layered on top of the
        /// host's <c>appsettings.json</c>. The base class always layers a
        /// <c>WorkingMode=WebhookViaServiceSetup</c> override (so the
        /// polling / webhook hosted services do not start) plus a dummy
        /// <c>BotToken</c> override.
        /// </param>
        protected TelegramWebhookScenarioBaseTest(
            ITelegramBotClient? botClientStub,
            IReadOnlyDictionary<string, string?>? extraConfiguration)
            : base(
                workingMode: TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup,
                botClientStub: botClientStub,
                extraConfiguration: extraConfiguration)
        {
        }
    }
}
