using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThingAppraiser.Core;
using ThingAppraiser.Crawlers.Tmdb;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.IO.Input.WebService;
using ThingAppraiser.IO.Output.WebService;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.ProcessingWebService.v1.Domain
{
    public sealed class ServiceRequestProcessor : IServiceRequestProcessor
    {
        public ServiceRequestProcessor()
        {
        }

        #region IServiceRequestProcessor Implementation

        public async Task<ProcessingResponse> ProcessRequest(RequestData requestData)
        {
            var builderDirector = Shell.CreateBuilderDirector(
                XmlConfigCreator.TransformConfigToXDocument(requestData.ConfigurationXml)
            );
            var shell = builderDirector.MakeShell();

            var inputTransmitter = new InputTransmitter(requestData.ThingNames);
            shell.InputManager.Add(inputTransmitter);

            var outputTransmitter = new OutputTransmitter();
            shell.OutputManager.Add(outputTransmitter);

            ServiceStatus status = await Task.Run(() => shell.Run("Processing response"));

            var results = outputTransmitter.GetResults();
            var response = new ProcessingResponse
            {
                Metadata = new ResponseMetadata
                {
                    CommonResultsNumber = results.Aggregate(
                        0, (counter, rating) => counter + rating.Count
                    ),
                    CommonResultCollectionsNumber = results.Count,
                    ResultStatus = status,
                    OptionalData = CreateOptionalData()
                },
                RatingDataContainers = results
            };
            return response;
        }

        #endregion

        private IReadOnlyDictionary<string, IOptionalData> CreateOptionalData()
        {
            var result = new Dictionary<string, IOptionalData>();
            if (TmdbServiceConfiguration.HasValue)
            {
                result.Add(nameof(TmdbServiceConfiguration),
                           TmdbServiceConfiguration.Configuration);
            }

            return result;
        }
    }
}
