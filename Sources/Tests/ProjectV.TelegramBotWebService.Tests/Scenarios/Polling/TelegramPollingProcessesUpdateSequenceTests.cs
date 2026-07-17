using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram;
using ProjectV.TelegramBotWebService.Tests.Scenarios.Helpers;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Polling
{
    /// <summary>
    /// Scenario TG-POLL-1: the production <c>PoolingProcessor</c> hosted
    /// service drains a fixed sequence of <see cref="Update" /> objects from
    /// the stubbed <c>ITelegramBotClient</c> and forwards each one
    /// through the full handler chain (<c>BotPollingUpdateHandler</c> →
    /// <c>UpdateService.HandleUpdateAsync</c> → <c>BotMessageHandler</c> →
    /// <c>IBotService.SendMessageAsync</c>).
    /// </summary>
    /// <remarks>
    /// The bot-client stub is a <see cref="StubTelegramBotClient" /> pre-loaded
    /// with the scripted update sequence — the first poll yields the configured
    /// updates, every subsequent poll yields an empty array, and the long-polling
    /// loop keeps running until the host shuts down when
    /// <c>WebApiBaseTest</c> disposal stops it after the test; the act phase
    /// only waits (with a bounded timeout) for the expected
    /// <c>SendMessageAsync</c> call count. The assertion proves
    /// the polling half of the Telegram coverage: the stub is consumed end-to-end
    /// by the polling hosted service and every update reaches
    /// <c>IBotService.SendMessageAsync</c>.
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class TelegramPollingProcessesUpdateSequenceTests
        : TelegramPollingScenarioBaseTest
    {
        // The scripted update sequence: three text-message updates with
        // commands the BotMessageHandler routes to SendMessageAsync. /start
        // and /help land on direct SendMessage replies; the freeform
        // "Hello there" lands on SendResponseToInvalidMessage (which is
        // also a SendMessage call). Net effect: three SendMessage calls
        // through the production handler chain.
        private const int ExpectedUpdateCount = 3;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramPollingProcessesUpdateSequenceTests" /> class.
        /// </summary>
        public TelegramPollingProcessesUpdateSequenceTests()
            : base(
                botClientStub: new StubTelegramBotClient(BuildUpdateSequence()))
        {
        }

        /// <summary>
        /// Scenario TG-POLL-1: the polling hosted service drains the scripted
        /// update sequence and the production handler chain calls
        /// <c>IBotService.SendMessageAsync</c> at least once per update.
        /// </summary>
        [Fact]
        public async Task PoolingProcessor_ProcessesFixedUpdateSequence_ForwardsToBotServiceSendMessage()
        {
            // Arrange.
            // The base ctor has already supplied the bot-client stub
            // (pre-loaded with three Updates via the stub's constructor) and
            // the bot-service stub. WebApiBaseTest.InitializeAsync
            // built the TestWebApplicationFactory and called CreateClient(),
            // which triggers IHost.StartAsync() — at this point the host has
            // resolved PoolingProcessor (BackgroundService) and is running
            // its receive loop in the background.
            //
            // The receive loop:
            //  1. Calls IBotService.DeleteWebhookAsync (stubbed → completes).
            //  2. Calls IBotService.BotClient.ReceiveAsync(handler, opts, ct).
            //  3. ReceiveAsync internally calls
            //     BotClient.SendRequest<Update[]>(new GetUpdatesRequest{...}, ct).
            //     The stub (StubTelegramBotClient pre-loaded in the ctor) yields
            //     the three updates on the first call and empty arrays
            //     thereafter.
            //  4. For each update, the receiver invokes
            //     BotPollingUpdateHandler.HandleUpdateAsync(client, update, ct)
            //     → UpdateService.HandleUpdateAsync(update, ct)
            //     → BotMessageHandler.ProcessAsync(message, ct)
            //     → IBotService.SendMessageAsync(chatId, text, ..., ct).

            // Act.
            // Wait for the polling loop to drain the scripted updates with
            // a bounded timeout — prevents the test from hanging if the
            // receive loop is misconfigured.
            using var timeoutSource = new CancellationTokenSource(
                TimeSpan.FromSeconds(15));
            await WaitForExpectedSendMessageCountAsync(
                ExpectedUpdateCount, timeoutSource.Token);

            // Assert.
            BotServiceStub.CalledMethodNames
                .Should()
                .NotBeEmpty(
                    "the polling loop must have forwarded at least one " +
                    "update through the production handler chain. " +
                    $"NLog captured: {string.Join(Environment.NewLine, CapturedException.LogLines)}");

            int sendMessageCallCount = CountSendMessageCalls();
            sendMessageCallCount.Should().BeGreaterThanOrEqualTo(
                ExpectedUpdateCount,
                $"the polling loop must drain all {ExpectedUpdateCount} scripted " +
                "updates and the production handler chain must call " +
                "IBotService.SendMessageAsync at least once per update. " +
                $"NLog captured: {string.Join(Environment.NewLine, CapturedException.LogLines)}");
        }

        // The Update sequence the scripted-bot-client yields on the first
        // poll. Three text-message updates with sequential Ids built via the
        // shared base-class helper. Every command lands on a
        // BotMessageHandler branch that calls IBotService.SendMessageAsync
        // exactly once.
        private static Update[] BuildUpdateSequence()
        {
            return new[]
            {
                BuildTextMessageUpdate(updateId: 100, messageId: 1, chatId: 999L, text: "/start"),
                BuildTextMessageUpdate(updateId: 101, messageId: 2, chatId: 999L, text: "/help"),
                BuildTextMessageUpdate(updateId: 102, messageId: 3, chatId: 999L, text: "Hello there"),
            };
        }

        private async Task WaitForExpectedSendMessageCountAsync(
            int target, CancellationToken cancellationToken)
        {
            // Poll the stub's call count until it reaches the target
            // or the cancellation token signals. The polling delay is small
            // because the receive loop runs in-process and is fast; the
            // bounded timeout (15 s) absorbs CI slowness without making the
            // test fragile on a fast machine.
            while (!cancellationToken.IsCancellationRequested)
            {
                if (CountSendMessageCalls() >= target)
                {
                    return;
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // Cancellation reached during the delay — fall through
                    // so the assertion can read the final count.
                    return;
                }
            }
        }

        private int CountSendMessageCalls()
        {
            // BotServiceStub.CalledMethodNames includes every IBotService call
            // (including the DeleteWebhookAsync call at the start of the polling
            // loop). Filter to SendMessageAsync so the count reflects only the
            // handler-chain end-state.
            return BotServiceStub.CalledMethodNames
                .Count(name => name == nameof(IBotService.SendMessageAsync));
        }
    }
}
