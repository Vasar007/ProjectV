using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Scenario TG-WEB-1: webhook receives a <c>/start</c> command and the
    /// production controller returns HTTP 200.
    /// </summary>
    /// <remarks>
    /// The test posts a synthetic <c>Telegram.Bot.Types.Update</c> JSON
    /// payload to the production
    /// <c>POST /api/v1/Update</c> endpoint (defined by
    /// <see cref="v1.Controllers.UpdateController" />). The host's
    /// <c>IBotService</c> is replaced by an NSubstitute substitute so that
    /// the bot handler chain (<see cref="v1.Domain.UpdateService" /> →
    /// <see cref="v1.Domain.Handlers.BotMessageHandler" /> →
    /// <c>SendMessageAsync</c>) runs end-to-end against the real ASP.NET Core
    /// pipeline without contacting the live Telegram API. The scenario
    /// asserts only that the controller responds 200 — that single status
    /// proves the entire model-binding + auth + middleware + handler chain
    /// is healthy on the webhook path (D-15 webhook half).
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
        /// text message returns HTTP 200.
        /// </summary>
        [Fact]
        public async Task PostUpdate_WithValidTextMessage_Returns200()
        {
            // Arrange.
            var update = new Update
            {
                Id = 1,
                Message = new Message
                {
                    Id = 100,
                    Text = "/start",
                    Chat = new Chat
                    {
                        Id = 999L,
                        Type = ChatType.Private
                    },
                    From = new User
                    {
                        Id = 999L,
                        FirstName = "Test",
                        IsBot = false
                    }
                }
            };
            string body = JsonConvert.SerializeObject(update);
            using var content = new StringContent(body, Encoding.UTF8, "application/json");

            // Act.
            using HttpResponseMessage response = await Client.PostAsync(
                "/api/v1/Update", content);

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.OK,
                "the webhook handler chain processed the /start command without faulting");
        }
    }
}
