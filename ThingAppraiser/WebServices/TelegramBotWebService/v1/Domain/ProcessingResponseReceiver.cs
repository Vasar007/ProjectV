using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public static class ProcessingResponseReceiver
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<UpdateServiceAsync>();


        public static void ScheduleRequest(IBotService botService, IServiceProxy serviceProxy,
            long chatId, RequestParams requestParams, CancellationToken token = default)
        {
            Task.Factory.StartNew(() =>
            {
                _logger.Info("Try to send request to service.");
                try
                {
                    var response = serviceProxy.SendPostRequest(requestParams).Result;
                    var converted = ConvertResultsToDict(response.RatingDataContainers);

                    _logger.Info("Got response. Output message to chat.");

                    botService.Client.SendTextMessageAsync(
                        chatId,
                        CreateResponseMessage(converted)
                    ).Wait();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception occurred during data processing request.");
                }
                _logger.Info("Request was processed.");
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private static Dictionary<string, List<double>> ConvertResultsToDict(
            List<List<RatingDataContainer>> results)
        {
            var converted = new Dictionary<string, List<double>>();
            foreach (List<RatingDataContainer> rating in results)
            {
                foreach (RatingDataContainer ratingDataContainer in rating)
                {
                    if (converted.TryGetValue(ratingDataContainer.DataHandler.Title,
                        out List<double> ratingValues))
                    {
                        ratingValues.Add(ratingDataContainer.RatingValue);
                    }
                    else
                    {
                        converted.Add(ratingDataContainer.DataHandler.Title,
                                      new List<double> { ratingDataContainer.RatingValue });
                    }
                }
            }
            return converted;
        }

        private static string CreateResponseMessage(Dictionary<string, List<double>> converted)
        {
            var stringBuilder = new StringBuilder(1000);
            stringBuilder.AppendLine("Results:");
            foreach (var (key, value) in converted)
            {
                stringBuilder.AppendLine(key + " — " + string.Join("| ", value));
            }
            return stringBuilder.ToString();
        }
    }
}
