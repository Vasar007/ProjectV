using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ThingAppraiser.Logging;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Core.Building;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public class UpdateServiceAsync : IUpdateServiceAsync
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<UpdateServiceAsync>();

        private readonly IBotService _botService;

        private readonly IServiceProxy _serviceProxy;

        private readonly ConcurrentDictionary<long, RequestParams> _cache;


        public UpdateServiceAsync(IBotService botService, IServiceProxy serviceProxy)
        {
            _botService = botService.ThrowIfNull(nameof(botService));
            _serviceProxy = serviceProxy.ThrowIfNull(nameof(serviceProxy));

            _cache = new ConcurrentDictionary<long, RequestParams>();
        }

        #region IUpdateService Implementation

        public async Task ProcessUpdateMessage(Update update)
        {
            if (update is null)
            {
                _logger.Warn("Received empty Message.");
                return;
            }
            
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    Message message = update.Message;
                    _logger.Info($"Received Message from {message.Chat.Id}.");
                    await ProcessMessage(message);
                    break;
                }

                default:
                {
                    _logger.Warn($"Skipped {update.Type}");
                    break;
                }
            }

        }

        #endregion

        private async Task ProcessMessage(Message message)
        {
            string[] data = message.Text.Split('\n');
            string[] firstLine = data.First().Split(' ');
            string command = firstLine.First();

            switch (command)
            {
                case "/services":
                {
                    await SendResponseToServicesCommand(message.Chat.Id);
                    break;
                }

                case "/request":
                {
                    await SendResponseToRequestCommand(message.Chat.Id);
                    break;
                }

                case "/cancel":
                {
                    await SendResponseToCancelCommand(message.Chat.Id);
                    break;
                }

                case "/help":
                {
                    await SendResponseToHelpCommand(message.Chat.Id);
                    break;
                }

                default:
                {
                    if (!_cache.TryGetValue(message.Chat.Id, out RequestParams requestParams))
                    {
                        await SendResponseToInvalidMessage(message.Chat.Id);
                        return;
                    }

                    if (requestParams.Requirements is null)
                    {
                        string serviceName = data.First();
                        await ContinueRequestCommandWithService(message.Chat.Id, serviceName,
                                                                requestParams);
                        return;
                    }

                    await ContinueRequestCommandWithData(message.Chat.Id, data, requestParams);
                    break;
                }
            }
        }

        private async Task SendResponseToHelpCommand(long chatId)
        {
            _logger.Info("Processes /help command.");

            const string usage = @"
Usage:
/services — send info about available services for processing;
/request — create a request to service;
/cancel — cancel the request;
/help — get help information.";

            await _botService.Client.SendTextMessageAsync(
                chatId,
                usage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToServicesCommand(long chatId)
        {
            _logger.Info("Processes /services command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                $"Available services: {string.Join(", ", ConfigContract.AvailableServices)}."
            );
        }

        private async Task SendResponseToRequestCommand(long chatId)
        {
            _logger.Info("Processes /request command.");

            var requestParams = new RequestParams();
            _cache.TryAdd(chatId, requestParams);

            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                ConfigContract.AvailableServices.ToArray(),
                new[] { "/cancel" },
            };

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Enter service name.",
                replyMarkup: replyKeyboard
            );
        }

        private async Task ContinueRequestCommandWithService(long chatId, string serviceName,
            RequestParams requestParams)
        {
            _logger.Info($"Continue process /request command with service {serviceName}.");
            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                ConfigContract.AvailableServices.ToArray(),
                new[] { "/cancel" },
            };

            if (!ConfigContract.AvailableServices.Contains(serviceName))
            {
                await _botService.Client.SendTextMessageAsync(
                    chatId,
                    "Invalid service name. Please, try again.",
                    replyMarkup: replyKeyboard
                );
                return;
            }

            requestParams.Requirements = CreateRequirements(serviceName, $"{serviceName}Common");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                $"Enter data for {serviceName}.",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task ContinueRequestCommandWithData(long chatId, string[] data,
            RequestParams requestParams)
        {
            _logger.Info("Continue process /request command with data.");

            requestParams.ThingNames = data.ToList();

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Send request to process data. Return later to see results.",
                replyMarkup: new ReplyKeyboardRemove()
            );

            ProcessingResponseReceiver.ScheduleRequest(_botService, _serviceProxy,
                                                       chatId, requestParams);

            _cache.TryRemove(chatId, out RequestParams _);
        }

        private async Task SendResponseToCancelCommand(long chatId)
        {
            _logger.Info("Processes /cancel command.");

            string message;
            if (_cache.TryRemove(chatId, out RequestParams _))
            {
                message = "Cancel the operation.";
            }
            else
            {
                message = "No request to cancel.";
            }

            await _botService.Client.SendTextMessageAsync(
                chatId,
                message,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToInvalidMessage(long chatId)
        {
            _logger.Info("Processes invalid message.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Invalid message. See usage at /help command.",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private ConfigRequirements CreateRequirements(string serviceName, string appraisalName)
        {
            serviceName.ThrowIfNullOrEmpty(nameof(serviceName));
            appraisalName.ThrowIfNullOrEmpty(nameof(appraisalName));

            IRequirementsCreator requirementsCreator = new RequirementsCreator();

            requirementsCreator.AddServiceRequirement(serviceName);
            requirementsCreator.AddAppraisalRequirement(appraisalName);

            return requirementsCreator.GetResult();
        }
    }
}
