using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Scenario TG-WEB-2: webhook rejects malformed JSON payloads.
    /// </summary>
    /// <remarks>
    /// The production webhook controller
    /// (<see cref="v1.Controllers.UpdateController.Post" />) is decorated
    /// with <c>[ApiController]</c> and accepts
    /// <c>[FromBody] Telegram.Bot.Types.Update</c>. When the body cannot be
    /// deserialised, the <c>[ApiController]</c> automatic-model-state
    /// validation short-circuits the pipeline with HTTP 400 before the
    /// action runs. The scenario asserts the response is a client error
    /// (4xx) — the exact status comes from the production
    /// <c>AddNewtonsoftJson</c> model binder, not from any code in the
    /// controller, so the test asserts the production behaviour AS-IS
    /// rather than dictating a specific 400 versus 415 outcome.
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class TelegramWebhookInvalidPayloadTests : TelegramWebhookScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TelegramWebhookInvalidPayloadTests" /> class.
        /// </summary>
        public TelegramWebhookInvalidPayloadTests()
        {
        }

        /// <summary>
        /// Scenario TG-WEB-2 — POST a malformed JSON body returns a 4xx
        /// client error (the auto-model-validation pipeline rejects the
        /// payload before the action runs).
        /// </summary>
        [Fact]
        public async Task PostUpdate_WithMalformedJson_ReturnsClientError()
        {
            // Arrange.
            const string malformedBody = "{ not valid json";
            using var content = new StringContent(malformedBody, Encoding.UTF8, "application/json");

            // Act.
            using HttpResponseMessage response = await Client.PostAsync(
                "/api/v1/Update", content);

            // Assert.
            int statusCode = (int) response.StatusCode;
            statusCode.Should().BeInRange(400, 499,
                "malformed JSON must be rejected by the production model binder " +
                "with a 4xx client error before the action runs");
        }
    }
}
