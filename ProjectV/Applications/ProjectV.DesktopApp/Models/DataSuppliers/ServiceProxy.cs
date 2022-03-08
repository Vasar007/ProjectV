﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.Models.WebService;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class ServiceProxy : IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxy>();

        private readonly string _baseAddress;

        private readonly string _apiUrl;

        private readonly HttpClient _client;


        public ServiceProxy()
        {
            _baseAddress = ConfigOptions.ProjectVService.CommunicationServiceBaseAddress;
            _apiUrl = ConfigOptions.ProjectVService.CommunicationServiceApiUrl;

            _logger.Info($"ProjectV service url: {_baseAddress}");

            _client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public async Task<ProcessingResponse?> SendPostRequest(RequestParams requestParams)
        {
            requestParams.ThrowIfNull(nameof(requestParams));

            _logger.Info($"Service method '{nameof(SendPostRequest)}' is called.");

            using HttpResponseMessage response = await _client.PostAsJsonAsync(
                _apiUrl, requestParams
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                return result;
            }

            return null;
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _client.Dispose();

            _disposed = true;
        }

        #endregion
    }
}
