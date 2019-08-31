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
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ProcessingResponseReceiver));


        public static void ScheduleRequest(IBotService botService, IServiceProxy serviceProxy,
            long chatId, RequestParams requestParams, CancellationToken token = default)
        {
            Task.Factory.StartNew(() =>
            {
                _logger.Info("Trying to send request to service.");
                try
                {
                    ProcessingResponse? response =
                        serviceProxy.SendPostRequest(requestParams).Result;

                    if (response is null)
                    {
                        throw new InvalidOperationException("Request has not successful status.");
                    }

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

        private static Dictionary<string, IList<double>> ConvertResultsToDict(
            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results)
        {
            var converted = new Dictionary<string, IList<double>>();
            foreach (IReadOnlyList<RatingDataContainer> rating in results)
            {
                foreach (RatingDataContainer ratingDataContainer in rating)
                {
                    if (converted.TryGetValue(ratingDataContainer.DataHandler.Title,
                                              out IList<double>? ratingValues))
                    {
                        if (ratingValues is null)
                        {
                            throw new InvalidOperationException(
                                "Rating data container contains null values"
                            );
                        }

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

        private static string CreateResponseMessage(Dictionary<string, IList<double>> converted)
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
