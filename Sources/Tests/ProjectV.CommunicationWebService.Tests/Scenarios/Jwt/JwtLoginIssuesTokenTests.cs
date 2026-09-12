using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization;
using ProjectV.Models.Users;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;
using Xunit;

namespace ProjectV.CommunicationWebService.Tests.Scenarios.Jwt
{
    /// <summary>
    /// Scenario JWT-3: <c>POST /api/v1/Users/login</c> with valid in-memory
    /// credentials issues a JWT pair.
    /// </summary>
    /// <remarks>
    /// The scenario seeds a single user into the in-memory user store via
    /// the production <see cref="IPasswordManager" /> (so the stored password
    /// salt and hash format match exactly what
    /// <c>UserService.LoginAsync</c> expects), POSTs credentials at
    /// <c>/api/v1/Users/login</c>, and asserts a 200 response with a non-null
    /// <c>AccessToken</c> on the deserialised <see cref="TokenResponse" />.
    /// The <c>ShouldCreateSystemUser</c> flag is held OFF to avoid the
    /// fire-and-forget seed race in <c>UserService</c>'s constructor — the
    /// scenario controls the entire user-store contents directly.
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class JwtLoginIssuesTokenTests : JwtAuthScenarioBaseTest
    {
        private const string TestUserName = "test-user-jwt-3";
        private const string TestPassword = "Sup3rS3cret-Test-Pwd-JWT-3";

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtLoginIssuesTokenTests" /> class.
        /// </summary>
        public JwtLoginIssuesTokenTests()
            : base(
                extraConfiguration: BuildExtraConfiguration(),
                configureTestServices: null)
        {
        }

        /// <inheritdoc />
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await SeedTestUserAsync(Factory.Services);
        }

        /// <summary>
        /// Scenario JWT-3 — login with valid in-memory credentials returns
        /// 200 + a token pair.
        /// </summary>
        [Fact]
        public async Task Login_WithValidInMemoryCredentials_ReturnsTokenResponse()
        {
            // Arrange.
            var request = new LoginRequest
            {
                UserName = TestUserName,
                Password = TestPassword
            };
            using var requestContent = JsonContent.Create(request);

            // Act.
            using HttpResponseMessage response = await Client.PostAsync(
                "/api/v1/Users/login", requestContent);

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.OK,
                "valid credentials must be accepted by the login endpoint");

            string payload = await response.Content.ReadAsStringAsync();
            payload.Should().NotBeNullOrWhiteSpace(
                "the login endpoint must return a non-empty body on success");

            using var doc = JsonDocument.Parse(payload);
            JsonElement root = doc.RootElement;

            // The CommunicationWebService MVC pipeline is wired with
            // AddNewtonsoftJson() which defaults to camelCase property
            // names — so look up both casings to stay robust against
            // future JSON-policy changes.
            JsonElement? accessTokenElement = FindPropertyAnyCase(root, "AccessToken");
            accessTokenElement.Should().NotBeNull(
                "TokenResponse JSON must expose an access-token property");

            accessTokenElement!.Value.ValueKind.Should().Be(JsonValueKind.Object,
                "AccessToken is an AccessTokenData record serialised as a JSON object");

            JsonElement? tokenElement = FindPropertyAnyCase(accessTokenElement.Value, "Token");
            tokenElement.Should().NotBeNull(
                "AccessToken must carry the serialised JWT string");

            tokenElement!.Value.ValueKind.Should().Be(JsonValueKind.String);
            tokenElement.Value.GetString().Should().NotBeNullOrWhiteSpace(
                "AccessToken.Token must be a non-empty signed JWT");
        }

        private static IReadOnlyDictionary<string, string?> BuildExtraConfiguration()
        {
            // Keep ShouldCreateSystemUser off so the UserService.ctor does not
            // race with the test seed; the test owns the entire in-memory
            // store.
            return new Dictionary<string, string?>
            {
                ["UserServiceOptions:AllowSignup"] = "false",
                ["UserServiceOptions:ShouldCreateSystemUser"] = "false",
                ["UserServiceOptions:CanUseSystemUserToAuthenticate"] = "false",
            };
        }

        private static async Task SeedTestUserAsync(IServiceProvider services)
        {
            var passwordManager = services.GetRequiredService<IPasswordManager>();
            var userInfoService = services.GetRequiredService<IUserInfoService>();

            byte[] salt = passwordManager.GetSecureSalt();
            Password hashedPassword = passwordManager.HashUsingPbkdf2(
                Password.Wrap(TestPassword), salt);

            var user = new UserInfo(
                id: UserId.Create(),
                userName: UserName.Wrap(TestUserName),
                password: hashedPassword,
                passwordSalt: Convert.ToBase64String(salt),
                creationTimeUtc: DateTime.UtcNow,
                active: true,
                refreshToken: null
            );

            await userInfoService.AddAsync(user);
        }

        private static JsonElement? FindPropertyAnyCase(JsonElement element, string propertyName)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value;
                }
            }

            return null;
        }
    }
}
