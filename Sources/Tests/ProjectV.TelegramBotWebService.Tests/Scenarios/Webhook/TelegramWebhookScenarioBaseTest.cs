using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using ProjectV.Core.Services.Clients;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Mocks.Core;
using ProjectV.Tests.Shared.Helpers.Mocks.Telegram;
using ProjectV.Tests.Shared.Helpers.WebApi;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Per-family base class for Telegram webhook scenario tests against
    /// <c>ProjectV.TelegramBotWebService</c>. Bundles the
    /// <see cref="TestWebApplicationFactory{TStartup}" /> wiring + the
    /// <see cref="IBotService" /> swap that every webhook scenario relies on.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The production <c>BotService</c> ctor constructs a real
    /// <see cref="ITelegramBotClient" /> via
    /// <c>new TelegramBotClient(BotToken, HttpClient)</c> — which throws on
    /// an empty <c>BotToken</c>. The base class therefore:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Removes the production
    ///   <see cref="IBotService" /> singleton from DI inside
    ///   <c>ConfigureTestServices</c>.</description></item>
    ///   <item><description>Re-registers <see cref="IBotService" /> as an
    ///   NSubstitute substitute whose <c>BotClient</c> property returns the
    ///   supplied <see cref="ITelegramBotClient" /> stub.</description></item>
    ///   <item><description>Sets
    ///   <c>TelegramBotWebServiceOptions:WorkingMode</c> to
    ///   <see cref="TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup" />
    ///   in the host's in-memory configuration so the host does NOT register
    ///   the <c>PoolingProcessor</c> / <c>ConfigureWebhook</c> hosted services
    ///   (both of which would resolve <see cref="IBotService" /> during host
    ///   startup, before our DI override has a chance to win).</description></item>
    ///   <item><description>Supplies a dummy non-empty <c>BotToken</c> so the
    ///   <c>BotOptions</c> validation chain (which runs lazily on first
    ///   <c>IOptions&lt;TelegramBotWebServiceOptions&gt;.Value</c> access) does
    ///   not blow up.</description></item>
    /// </list>
    /// <para>
    /// The bot client substitute is exposed as the protected
    /// <see cref="BotClientStub" /> so derived scenarios can assert on
    /// outgoing bot calls (e.g.
    /// <c>BotClientStub.Received().SendRequest(...)</c>) when relevant.
    /// </para>
    /// </remarks>
    public abstract class TelegramWebhookScenarioBaseTest : WebApiBaseTest<Startup>
    {
        /// <summary>
        /// Gets the <see cref="ITelegramBotClient" /> NSubstitute substitute
        /// the host's <see cref="IBotService" /> exposes via its
        /// <c>BotClient</c> property. Derived scenarios can assert on
        /// <c>BotClientStub.Received().SendRequest(...)</c> if they need to
        /// verify outgoing bot calls.
        /// </summary>
        protected ITelegramBotClient BotClientStub { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramWebhookScenarioBaseTest" /> class with default
        /// (bare, no-setup) bot-client substitute and no extra configuration
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
        /// Optional pre-built <see cref="ITelegramBotClient" /> substitute.
        /// When <c>null</c>, a bare
        /// <see cref="TestTelegramBotClientBuilder.CreateWithoutSetup" /> stub
        /// is used.
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
            : this(
                resolvedBotClientStub: new ResolvedStub(
                    botClientStub ?? TestTelegramBotClientBuilder.CreateWithoutSetup(BaseMockTest.CreateFixture())),
                extraConfiguration: extraConfiguration)
        {
        }

        // The private ctor takes a wrapper type so the overload resolution
        // is unambiguous and the bot-client stub is captured once + reused
        // both for the BotService substitute (passed through the
        // ConfigureTestServices delegate) and as the protected
        // BotClientStub property exposed to derived scenarios.
        private TelegramWebhookScenarioBaseTest(
            ResolvedStub resolvedBotClientStub,
            IReadOnlyDictionary<string, string?>? extraConfiguration)
            : base(
                jwtConfig: null,
                extraConfiguration: BuildConfiguration(extraConfiguration),
                configureTestServices: services => ConfigureBotServiceSwap(services, resolvedBotClientStub.Client))
        {
            BotClientStub = resolvedBotClientStub.Client;
        }

        // Tiny holder so the private ctor's signature does NOT collide with
        // the protected overload (which also accepts an ITelegramBotClient?
        // + extra-configuration pair).
        private readonly record struct ResolvedStub(ITelegramBotClient Client);

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            CapturedException.EnsureNLogMemoryTarget();
            CapturedException.Clear();
            return base.InitializeAsync();
        }

        private static void ConfigureBotServiceSwap(
            IServiceCollection services,
            ITelegramBotClient botClientStub)
        {
            services.RemoveAll<IBotService>();

            var botServiceSubstitute = Substitute.For<IBotService>();
            botServiceSubstitute.BotClient.Returns(botClientStub);
            services.AddSingleton(botServiceSubstitute);

            // The production CommunicationServiceClient's ctor instantiates
            // an HttpClient and validates RestApi/UserService options chain
            // — its inputs are not stable enough to construct during a
            // webhook integration test. Replace it with a no-setup
            // NSubstitute stub so any handler that resolves the client
            // does not blow up. Webhook scenarios do not assert on the
            // outgoing comm-client calls; polling scenarios will
            // pass a configured stub via the same factory knob.
            services.RemoveAll<ICommunicationServiceClient>();
            services.AddSingleton(TestCommunicationServiceClientBuilder.CreateWithoutSetup(BaseMockTest.CreateFixture()));
        }

        private static IReadOnlyDictionary<string, string?> BuildConfiguration(
            IReadOnlyDictionary<string, string?>? extra)
        {
            var merged = new Dictionary<string, string?>
            {
                // Force the host into a working mode that does NOT register
                // a hosted service that resolves IBotService at startup —
                // the swap in ConfigureTestServices fires after Startup
                // runs, so any service resolution before that point would
                // pull in the production BotService and explode on the
                // empty BotToken.
                ["TelegramBotWebServiceOptions:WorkingMode"] =
                    nameof(TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup),

                // Supply a non-empty dummy bot token so the BotOptions
                // validation chain doesn't blow up. The token is never
                // used because IBotService is replaced.
                ["TelegramBotWebServiceOptions:Bot:Token"] = "test-only-dummy-bot-token",
            };

            if (extra is not null)
            {
                foreach (var kvp in extra)
                {
                    merged[kvp.Key] = kvp.Value;
                }
            }

            return merged;
        }
    }
}
