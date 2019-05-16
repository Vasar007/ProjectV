using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThingAppraiser.Core;
using ThingAppraiser.Crawlers;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Models;
using ThingAppraiser.IO.Input.WebService;
using ThingAppraiser.IO.Output.WebService;

namespace ThingAppraiser.ProcessingWebService.v1.Domain
{
    public class ServiceRequestProcessor : IServiceRequestProcessor
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
                MetaData = new ResponseMetaData
                {
                    CommonResultsNumber = results.Aggregate(
                        0, (counter, rating) => counter + rating.Count
                    ),
                    CommonResultCollectionsNumber = results.Count,
                    ResultStatus = status,
                    OptionalData = new Dictionary<string, IOptionalData>
                    {
                        { nameof(TmdbServiceConfiguration), TmdbServiceConfiguration.Configuration }
                    }
                },
                RatingDataContainers = results
            };
            return response;
        }

        #endregion
    }
}
