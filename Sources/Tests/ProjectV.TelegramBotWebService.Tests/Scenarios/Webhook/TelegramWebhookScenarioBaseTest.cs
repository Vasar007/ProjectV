using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectV.Core.Services.Clients;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram;
using ProjectV.TelegramBotWebService.Tests.Scenarios.Helpers;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.Tests.Shared.ForTests;
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
    ///   <item><description>Re-registers <see cref="IBotService" /> as a
    ///   <see cref="StubBotService" /> concrete stub whose <c>BotClient</c>
    ///   property returns the supplied <see cref="ITelegramBotClient" />
    ///   stub.</description></item>
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
    /// The bot-client stub is exposed as the protected
    /// <see cref="BotClientStub" />, and the <see cref="StubBotService" />
    /// the host resolves is exposed as <see cref="BotServiceStub" /> so
    /// derived scenarios can assert on the production handler chain's
    /// downstream calls via <c>BotServiceStub.CalledMethodNames</c>.
    /// </para>
    /// </remarks>
    public abstract class TelegramWebhookScenarioBaseTest : WebApiBaseTest<Startup>
    {
        /// <summary>
        /// Gets the <see cref="ITelegramBotClient" /> stub the host's
        /// <see cref="IBotService" /> exposes via its <c>BotClient</c>
        /// property. Derived scenarios can cast to the concrete stub type
        /// and inspect its state if they need to verify outgoing bot calls.
        /// </summary>
        protected ITelegramBotClient BotClientStub { get; }

        /// <summary>
        /// Gets the <see cref="StubBotService" /> the host resolves in place
        /// of the production singleton. Derived scenarios can assert on
        /// <c>BotServiceStub.CalledMethodNames</c> to verify which
        /// <see cref="IBotService" /> methods the production handler chain
        /// invoked (e.g., that a webhook update produced a
        /// <c>SendMessageAsync</c> reply).
        /// </summary>
        protected StubBotService BotServiceStub { get; }

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
        /// <c>null</c>, a bare <see cref="StubTelegramBotClient" /> (no
        /// updates configured) is used.
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
                resolvedBotClientStub: new ResolvedBotStubs(
                    botClientStub ?? new StubTelegramBotClient()),
                extraConfiguration: extraConfiguration)
        {
        }

        // The private ctor takes a wrapper type so the overload resolution
        // is unambiguous and the bot-client + bot-service stubs are captured
        // once + reused both for the DI override (passed through the
        // ConfigureTestServices delegate) and as the protected
        // BotClientStub / BotServiceStub properties exposed to derived
        // scenarios.
        private TelegramWebhookScenarioBaseTest(
            ResolvedBotStubs resolvedBotClientStub,
            IReadOnlyDictionary<string, string?>? extraConfiguration)
            : base(
                jwtConfig: null,
                extraConfiguration: BuildConfiguration(extraConfiguration),
                configureTestServices: services =>
                    ConfigureBotServiceSwap(services, resolvedBotClientStub))
        {
            BotClientStub = resolvedBotClientStub.Client;
            BotServiceStub = resolvedBotClientStub.Service;
        }

        // Holder so the resolved bot-client + bot-service stubs can be
        // captured before the base ctor runs and re-used by both the
        // configureTestServices delegate and the protected properties.
        private readonly record struct ResolvedBotStubs(
            ITelegramBotClient Client,
            StubBotService Service)
        {
            public ResolvedBotStubs(ITelegramBotClient client)
                : this(client, BuildBotServiceStub(client))
            {
            }

            private static StubBotService BuildBotServiceStub(
                ITelegramBotClient client)
            {
                return new StubBotService(client);
            }
        }

        /// <inheritdoc />
        public override Task InitializeAsync()
        {
            CapturedException.EnsureNLogMemoryTarget();
            CapturedException.Clear();
            return base.InitializeAsync();
        }

        private static void ConfigureBotServiceSwap(
            IServiceCollection services,
            ResolvedBotStubs resolved)
        {
            services.RemoveAll<IBotService>();
            services.AddSingleton<IBotService>(resolved.Service);

            // The production CommunicationServiceClient's ctor instantiates
            // an HttpClient and validates RestApi/UserService options chain
            // — its inputs are not stable enough to construct during a
            // webhook integration test. Replace it with a concrete stub so
            // any handler that resolves the client does not blow up. Webhook
            // scenarios do not assert on the outgoing comm-client calls;
            // polling scenarios use the same pattern.
            services.RemoveAll<ICommunicationServiceClient>();
            services.AddSingleton<ICommunicationServiceClient>(new StubCommunicationServiceClient());
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
