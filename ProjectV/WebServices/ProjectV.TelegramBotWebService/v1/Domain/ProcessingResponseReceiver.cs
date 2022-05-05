﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProjectV.Core.Proxies;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.WebServices.Requests;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public static class ProcessingResponseReceiver
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ProcessingResponseReceiver));


        public static Task ScheduleRequestAsync(IBotService botService, IServiceProxy serviceProxy,
            long chatId, StartJobParamsRequest jobParams, CancellationToken token = default)
        {
            // Tricky code to send request in additional thread and transmit response to user.
            // Need to schedule task because our service should send response to Telegram.
            // Otherwise Telegram will retry to send event again until service send a response.
            return Task.Run(
                () => ScheduleRequestImplementation(botService, serviceProxy, chatId, jobParams),
                token
            );
        }

        private static async Task ScheduleRequestImplementation(IBotService botService,
            IServiceProxy serviceProxy, long chatId, StartJobParamsRequest jobParams)
        {
            _logger.Info("Trying to send request to ProjectV service.");

            try
            {
                var result = await serviceProxy.SendRequest(jobParams);

                if (!result.IsSuccess || result.Ok?.Metadata.ResultStatus != ServiceStatus.Ok)
                {
                    string? errorDetails = result.Error?.ErrorMessage ?? "Unknown error";
                    string errorMessage = $"Processing request to service failed: {errorDetails}.";
                    _logger.Error(errorMessage);
                    await botService.Client.SendTextMessageAsync(
                        chatId,
                        $"Cannot process request. Error: {errorMessage}"
                    );
                    return;
                }

                IReadOnlyDictionary<string, IList<double>> converted =
                    ConvertResultsToDict(result.Ok!.RatingDataContainers);

                _logger.Info("Got response. Output message to Telegram chat.");

                await botService.Client.SendTextMessageAsync(
                    chatId,
                    CreateResponseMessage(converted)
                );

                _logger.Info("Request was processed.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing request.");
                await botService.Client.SendTextMessageAsync(
                    chatId,
                    $"Cannot process request. Error: {ex.Message}"
                );
            }
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
