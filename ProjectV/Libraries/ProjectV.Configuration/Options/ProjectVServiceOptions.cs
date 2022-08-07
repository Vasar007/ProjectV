using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;

namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        [Required]
        public RestApiOptions RestApi { get; init; } = default!;

        public HttpClientOptions HttpClient { get; init; } = new();


        public ProjectVServiceOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            RestApi.ThrowIfNull(nameof(RestApi));
            HttpClient.ThrowIfNull(nameof(HttpClient));
        }

        #endregion
    }
}
