using System;
using System.Threading.Tasks;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core
{
    public sealed class CShellRx
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CShellRx>();

        private readonly Int32 _boundedCapacity;

        public IO.Input.CInputManagerRx InputManagerRx { get; }

        public Crawlers.CCrawlersManagerRx CrawlersManagerRx { get; }

        public Appraisers.CAppraisersManagerRx AppraisersManagerRx { get; }

        public IO.Output.COutputManagerRx OutputManagerRx { get; }


        public CShellRx(
            IO.Input.CInputManagerRx inputManagerRx,
            Crawlers.CCrawlersManagerRx crawlersManagerRx,
            Appraisers.CAppraisersManagerRx appraisersManagerRx,
            IO.Output.COutputManagerRx outputManagerRx,
            Int32 boundedCapacity)
        {
            InputManagerRx = inputManagerRx.ThrowIfNull(nameof(inputManagerRx));
            CrawlersManagerRx = crawlersManagerRx.ThrowIfNull(nameof(crawlersManagerRx));
            AppraisersManagerRx = appraisersManagerRx.ThrowIfNull(nameof(appraisersManagerRx));
            OutputManagerRx = outputManagerRx.ThrowIfNull(nameof(outputManagerRx));

            _boundedCapacity = boundedCapacity; // Not used now.
        }

        public EStatus Run(String storageName)
        {
            SGlobalMessageHandler.OutputMessage("Shell started work.");
            s_logger.Info("Shell started work.");

            // Input component work.
            IObservable<String> inputQueue =
                InputManagerRx.GetNames(storageName);

            // Crawlers component work.
            IObservable<CBasicInfo> responsesQueue =
                CrawlersManagerRx.CollectAllResponses(inputQueue);

            // Appraisers component work.
            IObservable<CRatingDataContainer> appraisersStatus =
                AppraisersManagerRx.GetAllRatings(responsesQueue);

            // Output component work.
            Task<Boolean> outputStatus =
                OutputManagerRx.SaveResults(appraisersStatus, String.Empty);

            outputStatus.Wait();

            // FIX ME: if there are error statuses need to create aggregate status which contains
            // more details then simple EStatus.Error value.
            Boolean success = outputStatus.Result;
            if (!success)
            {
                SGlobalMessageHandler.OutputMessage(
                    "Shell got error status during data processing."
                );
                s_logger.Info("Shell got error status during data processing.");
                return EStatus.Error;
            }

            s_logger.Info("Shell finished work successfully.");
            SGlobalMessageHandler.OutputMessage("Shell finished work successfully.");
            return EStatus.Ok;
        }
    }
}
