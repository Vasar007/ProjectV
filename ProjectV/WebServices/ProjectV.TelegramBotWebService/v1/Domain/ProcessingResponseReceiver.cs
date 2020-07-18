using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectV.Models.WebService;
using ProjectV.Logging;
using ProjectV.Models.Internal;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public static class ProcessingResponseReceiver
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ProcessingResponseReceiver));


        public static void ScheduleRequest(IBotService botService, IServiceProxy serviceProxy,
            long chatId, RequestParams requestParams, CancellationToken token = default)
        {
            // Tricky code to send request in additional thread and transmit response to user.
            // Need to schedule task because our service should send response to Telegram.
            // Otherwise Telegram will retry to send event again untill service send a response.
            Task.Run(
                () => ScheduleRequestImplementation(botService, serviceProxy,
                                                    chatId, requestParams),
                token
            );
        }

        private static async Task ScheduleRequestImplementation(IBotService botService,
            IServiceProxy serviceProxy, long chatId, RequestParams requestParams)
        {
            _logger.Info("Trying to send request to ProjectV service.");

            try
            {
                ProcessingResponse? response = await serviceProxy.SendPostRequest(requestParams);

                if (response is null)
                {
                    throw new InvalidOperationException("Request has not successful status.");
                }

                IReadOnlyDictionary<string, IList<double>> converted =
                    ConvertResultsToDict(response.RatingDataContainers);

                _logger.Info("Got response. Output message to Telegram chat.");

                await botService.Client.SendTextMessageAsync(
                    chatId,
                    CreateResponseMessage(converted)
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing request.");
                await botService.Client.SendTextMessageAsync(
                    chatId,
                    $"Cannot process request. Error: {ex.Message}"
                );
            }

            _logger.Info("Request was processed.");
        }

        private static IReadOnlyDictionary<string, IList<double>> ConvertResultsToDict(
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
                                "Rating data container contains null values."
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

        private static string CreateResponseMessage(
            IReadOnlyDictionary<string, IList<double>> converted)
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
