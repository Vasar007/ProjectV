using System;
using System.ComponentModel.DataAnnotations;
using ProjectV.Options;

namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        public RestApiOptions RestApi { get; set; } = default!;

        public HttpClientOptions HttpClient { get; set; } = default!;


        public ProjectVServiceOptions()
        {
        }
    }
}
