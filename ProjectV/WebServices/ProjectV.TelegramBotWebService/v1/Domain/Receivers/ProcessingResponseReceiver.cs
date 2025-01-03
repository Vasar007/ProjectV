﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Core.Services.Clients;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.WebServices.Requests;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Receivers
{
    public sealed class ProcessingResponseReceiver : IProcessingResponseReceiver
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingResponseReceiver>();

        private readonly ICommunicationServiceClient _serviceClient;


        public ProcessingResponseReceiver(
            ICommunicationServiceClient serviceClient)
        {
            _serviceClient = serviceClient.ThrowIfNull(nameof(serviceClient));
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _serviceClient.Dispose();

            _disposed = true;
        }

        #endregion

        #region IProcessingResponseReceiver Implementation

        public Task ScheduleRequestAsync(IBotService botService, long chatId,
            StartJobParamsRequest jobParams, CancellationToken cancellationToken)
        {
            // Tricky code to send request in additional thread and transmit response to user.
            // Need to schedule task because our service should send response to Telegram.
            // Otherwise Telegram will retry to send event again until service send a response.
            return Task.Run(
                () => ScheduleRequestImplementation(botService, chatId, jobParams, cancellationToken),
                cancellationToken
            );
        }

        #endregion

        private async Task ScheduleRequestImplementation(IBotService botService, long chatId,
            StartJobParamsRequest jobParams, CancellationToken cancellationToken)
        {
            _logger.Info("Trying to send request to ProjectV service.");

            try
            {
                var result = await _serviceClient.StartJobAsync(jobParams, cancellationToken);

                if (!result.IsSuccess || result.Ok?.Metadata.ResultStatus != ServiceStatus.Ok)
                {
                    string? errorDetails = result.Error?.ErrorMessage ?? "Unknown error.";
                    string errorMessage = GetProcessingErrorMessage(errorDetails);
                    _logger.Error(errorMessage);
                    await botService.SendMessageAsync(
                        chatId: chatId,
                        text: $"Cannot process request. Error: {errorMessage}",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                IReadOnlyDictionary<string, IList<double>> converted =
                    ConvertResultsToDict(result.Ok!.RatingDataContainers);

                _logger.Info("Got response. Output message to Telegram chat.");

                await botService.SendMessageAsync(
                    chatId: chatId,
                    text: CreateResponseMessage(converted),
                    cancellationToken: cancellationToken
                );

                _logger.Info("Request was processed.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing request.");
                await botService.SendMessageAsync(
                    chatId: chatId,
                    text: $"Cannot process request. Error: {ex.Message}",
                    cancellationToken: cancellationToken
                );
            }
        }

        private static string GetProcessingErrorMessage(string errorDetails)
        {
            const string errorMessageFormat = "Processing request to service failed: {0}{1}";

            if (errorDetails.EndsWith('.'))
            {
                return string.Format(errorMessageFormat, errorDetails, string.Empty);
            }

            return string.Format(errorMessageFormat, errorDetails, ".");
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
