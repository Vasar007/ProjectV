using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core
{
    public sealed class ShellRx
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ShellRx>();

        public IO.Input.InputManagerRx InputManagerRx { get; }

        public Crawlers.CrawlersManagerRx CrawlersManagerRx { get; }

        public Appraisers.AppraisersManagerRx AppraisersManagerRx { get; }

        public IO.Output.OutputManagerRx OutputManagerRx { get; }


        public ShellRx(
            IO.Input.InputManagerRx inputManagerRx,
            Crawlers.CrawlersManagerRx crawlersManagerRx,
            Appraisers.AppraisersManagerRx appraisersManagerRx,
            IO.Output.OutputManagerRx outputManagerRx,
            int _) // boundedCapacity not used now.
        {
            InputManagerRx = inputManagerRx.ThrowIfNull(nameof(inputManagerRx));
            CrawlersManagerRx = crawlersManagerRx.ThrowIfNull(nameof(crawlersManagerRx));
            AppraisersManagerRx = appraisersManagerRx.ThrowIfNull(nameof(appraisersManagerRx));
            OutputManagerRx = outputManagerRx.ThrowIfNull(nameof(outputManagerRx));
        }

        public static Building.ShellRxBuilderDirector CreateBuilderDirector(
            XDocument configuration)
        {
            return new Building.ShellRxBuilderDirector(
                new Building.ShellRxBuilderFromXDocument(configuration)
            );
        }

        public async Task<ServiceStatus> Run(string storageName)
        {
            GlobalMessageHandler.OutputMessage("Shell started work.");
            _logger.Info("Shell started work.");

            // Input component work.
            IObservable<string> inputQueue =
                InputManagerRx.GetNames(storageName);

            // Crawlers component work.
            IDictionary<Type, IObservable<BasicInfo>> responsesQueues =
                CrawlersManagerRx.CollectAllResponses(inputQueue);

            // Appraisers component work.
            IDictionary<Type, IObservable<RatingDataContainer>> ratingsQueues =
                AppraisersManagerRx.GetAllRatings(responsesQueues);

            // Output component work.
            bool outputStatus =
                await OutputManagerRx.SaveResults(ratingsQueues, string.Empty);

            // FIX ME: if there are error statuses need to create aggregate status which contains
            // more details then simple EStatus.Error value.
            if (!outputStatus)
            {
                GlobalMessageHandler.OutputMessage(
                    "Shell got error status during data processing."
                );
                _logger.Info("Shell got error status during data processing.");
                return ServiceStatus.Error;
            }

            _logger.Info("Shell finished work successfully.");
            GlobalMessageHandler.OutputMessage("Shell finished work successfully.");
            return ServiceStatus.Ok;
        }
    }
}
