using System;
using System.Threading;
using System.Threading.Tasks;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.Tests.Shared.Helpers.Mocks.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Polling
{
    /// <summary>
    /// Scenario TG-POLL-1: the production <c>PoolingProcessor</c> hosted
    /// service drains a fixed sequence of <see cref="Update" /> objects from
    /// the substituted <c>ITelegramBotClient</c> and forwards each one
    /// through the full handler chain (<c>BotPollingUpdateHandler</c> →
    /// <c>UpdateService.HandleUpdateAsync</c> → <c>BotMessageHandler</c> →
    /// <c>IBotService.SendMessageAsync</c>).
    /// </summary>
    /// <remarks>
    /// The bot-client substitute is built via
    /// <see cref="TestTelegramBotClientBuilder.WithUpdateSequence(System.Collections.Generic.IEnumerable{Update})" />
    /// — the first poll yields the configured updates, every subsequent poll
    /// yields an empty array, and the long-polling loop exits when the host's
    /// cancellation token signals (the test stops the host explicitly inside
    /// the act-phase polling loop). The assertion proves the polling half
    /// of the Telegram coverage: the <c>WithUpdateSequence(...)</c> builder
    /// is consumed end-to-end by the polling hosted service and every
    /// update reaches <c>IBotService.SendMessageAsync</c>.
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
                botClientStub: new TestTelegramBotClientBuilder()
                    .WithUpdateSequence(BuildUpdateSequence())
                    .Build())
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
            // The base ctor has already supplied the bot-client substitute
            // (pre-loaded with three Updates via WithUpdateSequence) and
            // the bot-service substitute. WebApiBaseTest.InitializeAsync
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
            //     The substitute (configured via WithUpdateSequence) yields
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
            BotServiceStub.ReceivedCalls()
                .Should()
                .NotBeEmpty(
                    "the polling loop must have forwarded at least one " +
                    "update through the production handler chain. " +
                    $"NLog captured: {string.Join(Environment.NewLine, Webhook.CapturedException.LogLines)}");

            int sendMessageCallCount = CountSendMessageCalls();
            sendMessageCallCount.Should().BeGreaterThanOrEqualTo(
                ExpectedUpdateCount,
                $"the polling loop must drain all {ExpectedUpdateCount} scripted " +
                "updates and the production handler chain must call " +
                "IBotService.SendMessageAsync at least once per update. " +
                $"NLog captured: {string.Join(Environment.NewLine, Webhook.CapturedException.LogLines)}");
        }

        // The Update sequence the scripted-bot-client yields on the first
        // poll. Three text-message updates with sequential Ids. Every
        // command lands on a BotMessageHandler branch that calls
        // IBotService.SendMessageAsync exactly once.
        private static Update[] BuildUpdateSequence()
        {
            return new[]
            {
                BuildTextMessageUpdate(updateId: 100, messageId: 1, chatId: 999L, text: "/start"),
                BuildTextMessageUpdate(updateId: 101, messageId: 2, chatId: 999L, text: "/help"),
                BuildTextMessageUpdate(updateId: 102, messageId: 3, chatId: 999L, text: "Hello there"),
            };
        }

        private static Update BuildTextMessageUpdate(
            int updateId, int messageId, long chatId, string text)
        {
            return new Update
            {
                Id = updateId,
                Message = new Message
                {
                    Id = messageId,
                    Text = text,
                    Chat = new Chat
                    {
                        Id = chatId,
                        Type = ChatType.Private
                    },
                    From = new User
                    {
                        Id = chatId,
                        FirstName = "Test",
                        IsBot = false
                    }
                }
            };
        }

        private async Task WaitForExpectedSendMessageCountAsync(
            int target, CancellationToken cancellationToken)
        {
            // Poll the substitute's call count until it reaches the target
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
            // BotServiceStub.ReceivedCalls() iterates every NSubstitute call
            // (including the DeleteWebhookAsync call). Filter to the
            // SendMessageAsync method so the count reflects only the
            // handler-chain end-state.
            int count = 0;
            foreach (var call in BotServiceStub.ReceivedCalls())
            {
                if (call.GetMethodInfo().Name ==
                    nameof(BotServiceStub.SendMessageAsync))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
