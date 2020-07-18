using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectV.Core;
using ProjectV.Configuration;
using ProjectV.IO.Input.WebService;
using ProjectV.IO.Output.WebService;
using ProjectV.Models.Internal;
using ProjectV.Models.WebService;
using ProjectV.TmdbService;

namespace ProjectV.ProcessingWebService.v1.Domain
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
                    CommonResultsNumber = results.Sum(rating => rating.Count),
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
