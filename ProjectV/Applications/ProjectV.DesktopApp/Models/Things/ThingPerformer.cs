using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Linq;
using ProjectV.Building;
using ProjectV.Building.Service;
using ProjectV.Configuration;
using ProjectV.Configuration.Options;
using ProjectV.Core.Proxies;
using ProjectV.DesktopApp.Domain;
using ProjectV.DesktopApp.Domain.Executor;
using ProjectV.IO.Input.File;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;

namespace ProjectV.DesktopApp.Models.Things
{
    internal sealed class ThingPerformer : IPerformer<ThingPerformerInfo, ThingResultInfo>,
        IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ThingPerformer>();

        private readonly IRequirementsCreator _requirementsCreator;

        private readonly IProxyClient _serviceProxyClient;


        public ThingPerformer(
            IHttpClientFactory httpClientFactory,
            ProjectVServiceOptions serviceOptions,
            UserServiceOptions userServiceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));
            userServiceOptions.ThrowIfNull(nameof(userServiceOptions));

            _requirementsCreator = new RequirementsCreator();
            _serviceProxyClient = new CommunicationProxyClient(
                httpClientFactory, serviceOptions, userServiceOptions
            );
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _serviceProxyClient.Dispose();

            _disposed = true;
        }

        #endregion

        #region IPerformer<ThingPerformerInfo, ThingResultInfo> Implementation

        public async Task<ThingResultInfo> PerformAsync(ThingPerformerInfo thingsInfo)
        {
            thingsInfo.ThrowIfNull(nameof(thingsInfo));

            var requestParams = await ConfigureServiceRequest(thingsInfo)
                .ConfigureAwait(continueOnCapturedContext: false);

            var result = await _serviceProxyClient.StartJobAsync(requestParams)
                .ConfigureAwait(continueOnCapturedContext: false);

            return new ThingResultInfo(thingsInfo.ServiceName, result);
        }

        #endregion

        private async Task<StartJobParamsRequest> ConfigureServiceRequest(
            ThingPerformerInfo thingsInfo)
        {
            return thingsInfo.Data.DataSource switch
            {
                DataSource.Nothing => throw new InvalidOperationException(
                                          "Data source was not set."
                                      ),

                DataSource.InputThing => CreateRequestWithUserInputData(
                                             thingsInfo.ServiceName, thingsInfo.Data.ThingNames
                                         ),

                DataSource.LocalFile => await CreateRequestWithLocalFileData(
                                            thingsInfo.ServiceName, thingsInfo.Data.StorageName
                                        ).ConfigureAwait(continueOnCapturedContext: false),

                DataSource.GoogleDrive => await CreateGoogleDriveRequest(
                                              thingsInfo.ServiceName, thingsInfo.Data.StorageName
                                          ).ConfigureAwait(continueOnCapturedContext: false),

                _ => throw new ArgumentOutOfRangeException(
                         nameof(thingsInfo), thingsInfo.Data.DataSource,
                         "Could not recognize specified data source type: " +
                         $"\"{thingsInfo.Data.DataSource.ToString()}\"."
                     )
            };
        }

        private static void ThrowIfDataIsInvalid(IReadOnlyCollection<string> data)
        {
            const int minItemsNumberToAppraise = 2;
            if (data.IsNullOrEmpty() || data.Count < minItemsNumberToAppraise)
            {
                throw new InvalidOperationException("Insufficient amount of data to be processed.");
            }
        }

        private StartJobParamsRequest CreateRequestWithUserInputData(string serviceName,
            IReadOnlyList<string> thingNames)
        {
            _logger.Info($"Perform request with user input data for service \"{serviceName}\". " +
                         $"Number of things names: \"{thingNames.Count.ToString()}\"");

            CreateBasicRequirements(serviceName);

            ThrowIfDataIsInvalid(thingNames);
            return new StartJobParamsRequest
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<StartJobParamsRequest> CreateRequestWithLocalFileData(string serviceName,
            string storageName)
        {
            _logger.Info($"Perform request with local file data for service \"{serviceName}\". " +
                         $"Storage name: \"{storageName}\".");

            CreateBasicRequirements(serviceName);

            // Read local file, retrieve all things and send them as list to service to crawling
            // and appraise.
            var localFileReader = new LocalFileReader(new SimpleFileReader());
            IReadOnlyList<string> thingNames = await Task
                .Run(() => localFileReader.ReadThingNames(storageName).ToReadOnlyList())
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfDataIsInvalid(thingNames);
            return new StartJobParamsRequest
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<StartJobParamsRequest> CreateGoogleDriveRequest(string serviceName,
            string storageName)
        {
            _logger.Info($"Perform request with google drive data for service \"{serviceName}\". " +
                         $"Storage name: \"{storageName}\".");

            CreateBasicRequirements(serviceName);

            // Read file from Google Drive, retrieve all things and send them as list to service to
            // crawling and appraise.
            var serviceBuilder = new ServiceBuilderForXmlConfig();
            var googleDriveReader = serviceBuilder.CreateInputter(
                ConfigModule.GetConfigForInputter(ConfigNames.Inputters.GoogleDriveReaderSimpleName)
            );

            IReadOnlyList<string> thingNames = await Task
                .Run(() => googleDriveReader.ReadThingNames(storageName).ToReadOnlyList())
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfDataIsInvalid(thingNames);
            return new StartJobParamsRequest
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        // TODO: we can use local requirements creator instead of reusing the old one.
        private void CreateBasicRequirements(string serviceName)
        {
            serviceName = ConfigContract.GetProperServiceName(serviceName);

            _requirementsCreator.Reset();
            _requirementsCreator.AddServiceRequirement(serviceName);
            _requirementsCreator.AddAppraisalRequirement($"{serviceName}Common");
        }
    }
}
