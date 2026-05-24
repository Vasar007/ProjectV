using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectV.Core.Services.Clients;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram;
using ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Mocks.Core;
using ProjectV.Tests.Shared.Helpers.Mocks.Telegram;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Polling
{
    /// <summary>
    /// Per-family base class for Telegram polling scenario tests against
    /// <c>ProjectV.TelegramBotWebService</c>. Sibling to
    /// <see cref="TelegramWebhookScenarioBaseTest" />.
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
    /// loop. Second, the test asserts on the in-process call-count recorded by
    /// the <see cref="StubBotService" /> (one <c>SendMessageAsync</c> call per
    /// update the production handler chain drains) rather than on an HTTP
    /// response, because the polling path has no outbound HTTP response
    /// surface.
    /// </para>
    /// <para>
    /// Like the webhook base, this base class:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Removes the production
    ///   <see cref="IBotService" /> singleton and re-registers a
    ///   <see cref="StubBotService" /> whose <c>BotClient</c> property returns
    ///   the supplied <see cref="ITelegramBotClient" /> stub. The stub handles
    ///   <c>DeleteWebhookAsync</c> (no-op) and records every
    ///   <c>SendMessageAsync</c> invocation in
    ///   <see cref="StubBotService.CalledMethodNames" /> so the handler-chain
    ///   assertion can read the call count deterministically without
    ///   NSubstitute.</description></item>
    ///   <item><description>Removes the production
    ///   <see cref="ICommunicationServiceClient" /> transient and re-registers a
    ///   no-setup
    ///   <see cref="TestCommunicationServiceClientBuilder.CreateWithoutSetup" />
    ///   substitute. The polling path's
    ///   <c>BotMessageHandler</c> branch for non-<c>/request</c> commands does
    ///   not reach the comm-client, but the DI graph wires it transitively so
    ///   the production <c>CommunicationServiceClient</c> ctor (which
    ///   validates a strict options chain) must be kept out of the test path.</description></item>
    ///   <item><description>Sets the host's
    ///   <c>TelegramBotWebServiceOptions:WorkingMode</c> to
    ///   <see cref="TelegramBotWebServiceWorkingMode.PollingViaHostedService" />
    ///   so the host registers and starts <c>PoolingProcessor</c> as a
    ///   <c>BackgroundService</c> when <see cref="WebApiBaseTest{TStartup}.Client" />
    ///   is built. The <c>PoolingProcessor</c> factory
    ///   (<c>PoolingProcessor.Create</c>) resolves <c>IBotPolling</c> from the
    ///   container at host start; <c>BotPolling</c>'s ctor pulls
    ///   <c>IBotService</c>, which by then is the test-side stub (the test
    ///   override registered in <c>ConfigureTestServices</c> runs AFTER
    ///   <c>Startup.ConfigureServices</c> but BEFORE the host starts its
    ///   <c>IHostedService</c> instances, so the substitution wins).</description></item>
    ///   <item><description>Supplies a non-empty dummy <c>Bot:Token</c> so
    ///   <c>BotOptions.Validate()</c> passes on first
    ///   <c>IOptions&lt;TelegramBotWebServiceOptions&gt;.Value</c> access. The
    ///   dummy token is never used because <c>IBotService</c> is replaced.</description></item>
    /// </list>
    /// <para>
    /// The bot-client stub is exposed as <see cref="BotClientStub" /> so
    /// derived scenarios can build it via
    /// <see cref="TestTelegramBotClientBuilder.WithUpdateSequence(System.Collections.Generic.IEnumerable{global::Telegram.Bot.Types.Update})" />
    /// and assert on outgoing <c>SendRequest</c> calls if needed. The
    /// <see cref="StubBotService" /> is exposed as <see cref="BotServiceStub" />
    /// so scenarios can assert on the production handler chain's downstream
    /// calls via <c>BotServiceStub.CalledMethodNames</c>.
    /// </para>
    /// </remarks>
    public abstract class TelegramPollingScenarioBaseTest : WebApiBaseTest<Startup>
    {
        /// <summary>
        /// Gets the <see cref="ITelegramBotClient" /> stub the host's
        /// <see cref="IBotService" /> exposes via its <c>BotClient</c>
        /// property. Typically built via
        /// <see cref="TestTelegramBotClientBuilder.WithUpdateSequence(System.Collections.Generic.IEnumerable{global::Telegram.Bot.Types.Update})" />
        /// in the derived ctor.
        /// </summary>
        protected ITelegramBotClient BotClientStub { get; }

        /// <summary>
        /// Gets the <see cref="StubBotService" /> the host resolves in place
        /// of the production singleton. Derived scenarios can assert on
        /// <c>BotServiceStub.CalledMethodNames</c> to verify which
        /// <see cref="IBotService" /> methods the production handler chain
        /// invoked and how many times (e.g., count entries equal to
        /// <c>nameof(IBotService.SendMessageAsync)</c> to confirm the
        /// expected number of updates were drained).
        /// </summary>
        protected StubBotService BotServiceStub { get; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramPollingScenarioBaseTest" /> class.
        /// </summary>
        /// <param name="botClientStub">
        /// Optional pre-built <see cref="ITelegramBotClient" /> substitute.
        /// When <c>null</c>, a bare
        /// <see cref="TestTelegramBotClientBuilder.CreateWithoutSetup" /> stub
        /// is used (the polling loop will fetch an empty batch on the first
        /// call and keep looping until cancellation — useful only for tests
        /// that do not assert on update consumption).
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
            : this(
                resolvedBotClientStub: new ResolvedBotStubs(
                    botClientStub ?? TestTelegramBotClientBuilder.CreateWithoutSetup(BaseMockTest.CreateFixture())),
                extraConfiguration: extraConfiguration)
        {
        }

        // Tiny wrapper so the protected ctor's signature does not collide with
        // a hypothetical future overload and so we can capture the supplied
        // (or default) bot-client + freshly-built bot-service substitutes
        // exactly once for both the DI override AND the protected properties.
        private TelegramPollingScenarioBaseTest(
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

            // Same rationale as the webhook base class: the production
            // CommunicationServiceClient validates a strict options chain
            // that is not satisfied by the test configuration. The polling
            // BotMessageHandler branches we exercise do not reach the
            // comm-client, but a transient dep of UpdateService /
            // BotMessageHandler resolves it eagerly when the singleton
            // graph is built.
            services.RemoveAll<ICommunicationServiceClient>();
            services.AddSingleton(TestCommunicationServiceClientBuilder.CreateWithoutSetup(BaseMockTest.CreateFixture()));
        }

        private static IReadOnlyDictionary<string, string?> BuildConfiguration(
            IReadOnlyDictionary<string, string?>? extra)
        {
            var merged = new Dictionary<string, string?>
            {
                // Polling-via-hosted-service is the working mode under test.
                // The host registers PoolingProcessor as a BackgroundService
                // and starts it when IHost.StartAsync() runs (which the
                // WebApplicationFactory triggers from CreateClient()).
                ["TelegramBotWebServiceOptions:WorkingMode"] =
                    nameof(TelegramBotWebServiceWorkingMode.PollingViaHostedService),

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
