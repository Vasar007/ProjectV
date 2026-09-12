using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Newtonsoft.Json;
using ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot.Types;
using Xunit;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Scenario TG-WEB-1: webhook receives a <c>/start</c> command, the
    /// production controller returns HTTP 200, and the handler chain sends
    /// a reply through <see cref="IBotService.SendMessageAsync" />.
    /// </summary>
    /// <remarks>
    /// The test posts a synthetic <c>Telegram.Bot.Types.Update</c> JSON
    /// payload to the production
    /// <c>POST /api/v1/Update</c> endpoint (defined by
    /// <see cref="v1.Controllers.UpdateController" />). The host's
    /// <c>IBotService</c> is replaced by a concrete
    /// <see cref="StubBotService" /> so that the bot handler chain
    /// (<see cref="v1.Domain.UpdateService" /> →
    /// <see cref="v1.Domain.Handlers.BotMessageHandler" /> →
    /// <c>SendMessageAsync</c>) runs end-to-end against the real ASP.NET Core
    /// pipeline without contacting the live Telegram API. The scenario
    /// asserts the controller responds 200 AND that the handler chain
    /// actually produced a reply — the stub records every
    /// <see cref="IBotService" /> call in
    /// <see cref="StubBotService.CalledMethodNames" />, so a chain that
    /// silently dropped the message would fail the second assertion.
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class TelegramWebhookTextMessageTests : TelegramWebhookScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramWebhookTextMessageTests" /> class.
        /// </summary>
        public TelegramWebhookTextMessageTests()
        {
        }

        /// <summary>
        /// Scenario TG-WEB-1 — POST a valid Update with a <c>/start</c>
        /// text message returns HTTP 200 and sends a reply through
        /// <see cref="IBotService.SendMessageAsync" />.
        /// </summary>
        [Fact]
        public async Task PostUpdate_WithValidTextMessage_Returns200AndSendsReply()
        {
            // Arrange.
            Update update = BuildTextMessageUpdate(
                updateId: 1, messageId: 100, chatId: 999L, text: "/start");
            string body = JsonConvert.SerializeObject(update);
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act.
            using HttpResponseMessage response = await Client.PostAsync(
                "/api/v1/Update", content);

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.OK,
                "the webhook handler chain processed the /start command without faulting");
            BotServiceStub.CalledMethodNames.Should().Contain(
                nameof(IBotService.SendMessageAsync),
                "the /start command must reach BotMessageHandler and produce " +
                "a reply — a handler chain that silently dropped the message " +
                "would still return 200");
        }
    }
}
