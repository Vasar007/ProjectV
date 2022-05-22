using System.ComponentModel.DataAnnotations;

namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        [Required]
        public RestApiOptions RestApi { get; set; } = default!;

        public HttpClientOptions HttpClient { get; set; } = new();


        public ProjectVServiceOptions()
        {
        }
    }
}
