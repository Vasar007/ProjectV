using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Collections;
using ThingAppraiser.Building;
using ThingAppraiser.Building.Service;
using ThingAppraiser.Configuration;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Executor;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.IO.Input.File;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal sealed class ThingPerformer : IPerformer<ThingPerformerInfo, ThingResultInfo>
    {
        private static readonly ILogger _logger =
               LoggerFactory.CreateLoggerFor<ThingPerformer>();

        private readonly IRequirementsCreator _requirementsCreator;

        private readonly ServiceProxy _serviceProxy;


        public ThingPerformer()
        {
            _requirementsCreator = new RequirementsCreator();
            _serviceProxy = new ServiceProxy();
        }

        #region IPerformer<ThingPerformerInfo, ThingResultInfo> Implementation

        public async Task<ThingResultInfo> PerformAsync(ThingPerformerInfo thingsInfo)
        {
            thingsInfo.ThrowIfNull(nameof(thingsInfo));

            RequestParams requestParams = await ConfigureServiceRequest(thingsInfo)
                .ConfigureAwait(continueOnCapturedContext: false);

            ProcessingResponse? response = await _serviceProxy.SendPostRequest(requestParams)
                .ConfigureAwait(continueOnCapturedContext: false);

            return new ThingResultInfo(thingsInfo.ServiceName, response);
        }

        #endregion

        private async Task<RequestParams> ConfigureServiceRequest(ThingPerformerInfo thingsInfo)
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

        private RequestParams CreateRequestWithUserInputData(string serviceName,
            IReadOnlyList<string> thingNames)
        {
            _logger.Info($"Perform request with user input data for service \"{serviceName}\". " +
                         $"Number of things names: \"{thingNames.Count.ToString()}\"");

            CreateBasicRequirements(serviceName);

            ThrowIfDataIsInvalid(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<RequestParams> CreateRequestWithLocalFileData(string serviceName,
            string storageName)
        {
            _logger.Info($"Perform request with local file data for service \"{serviceName}\". " +
                         $"Storage name: \"{storageName}\".");

            CreateBasicRequirements(serviceName);

            var localFileReader = new LocalFileReader(new SimpleFileReader());
            IReadOnlyList<string> thingNames = await Task
                .Run(() => localFileReader.ReadThingNames(storageName))
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfDataIsInvalid(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<RequestParams> CreateGoogleDriveRequest(string serviceName,
            string storageName)
        {
            _logger.Info($"Perform request with google drive data for service \"{serviceName}\". " +
                         $"Storage name: \"{storageName}\".");

            CreateBasicRequirements(serviceName);

            var serviceBuilder = new ServiceBuilderForXmlConfig();
            var googleDriveReader = serviceBuilder.CreateInputter(
                ConfigModule.GetConfigForInputter(ConfigNames.Inputters.GoogleDriveReaderSimpleName)
            );

            IReadOnlyList<string> thingNames = await Task
                .Run(() => googleDriveReader.ReadThingNames(storageName))
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfDataIsInvalid(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private void CreateBasicRequirements(string serviceName)
        {
            serviceName = ConfigContract.GetProperServiceName(serviceName);

            _requirementsCreator.Reset();
            _requirementsCreator.AddServiceRequirement(serviceName);
            _requirementsCreator.AddAppraisalRequirement($"{serviceName}Common");
        }
    }
}
