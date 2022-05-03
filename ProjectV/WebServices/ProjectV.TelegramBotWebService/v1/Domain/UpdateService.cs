#pragma warning disable format // dotnet format fails indentation for switch :(

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Building;
using ProjectV.Configuration;
using ProjectV.IO.Input;
using ProjectV.Logging;
using ProjectV.Models.WebService.Requests;
using ProjectV.TelegramBotWebService.Properties;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Cache;
using ProjectV.TelegramBotWebService.v1.Domain.Proxy;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public sealed class UpdateService : IUpdateService
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<UpdateService>();

        // TODO: add settings to configure bot commands.

        private readonly IBotService _botService;

        private readonly IServiceProxy _serviceProxy;

        private readonly IUserCache _userCache;

        private readonly ITelegramTextProcessor _textProcessor;


        public UpdateService(
            IBotService botService,
            IServiceProxy serviceProxy,
            IUserCache userCache,
            ITelegramTextProcessor textProcessor)
        {
            _botService = botService.ThrowIfNull(nameof(botService));
            _serviceProxy = serviceProxy.ThrowIfNull(nameof(serviceProxy));
            _userCache = userCache.ThrowIfNull(nameof(userCache));
            _textProcessor = textProcessor.ThrowIfNull(nameof(textProcessor));
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
                    string encodedMessage = UserInputEncoder.Encode($"Skipped {update.Type}.");
                    _logger.Warn(encodedMessage);
                    break;
                }
            }

        }

        #endregion

        private async Task ProcessMessage(Message message)
        {
            IReadOnlyList<string> data = _textProcessor.ParseAsSeparateLines(message.Text);
            string command = _textProcessor.ParseCommand(data[0]);

            switch (command)
            {
                case "/start":
                {
                    await SendResponseToStartCommand(message.Chat.Id);
                    break;
                }

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
                    if (!_userCache.TryGetUser(message.Chat.Id, out StartJobParamsRequest? jobData))
                    {
                        await SendResponseToInvalidMessage(message.Chat.Id);
                        return;
                    }

                    if (jobData.Requirements is null)
                    {
                        string serviceName = data[0];
                        await ContinueRequestCommandWithService(message.Chat.Id, serviceName,
                                                                jobData);
                        return;
                    }

                    await ContinueRequestCommandWithData(message.Chat.Id, data, jobData);
                    break;
                }
            }
        }

        private async Task SendResponseToStartCommand(long chatId)
        {
            _logger.Info("Processes /start command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                Messages.HelloMessage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToHelpCommand(long chatId)
        {
            _logger.Info("Processes /help command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                Messages.HelpMessage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToServicesCommand(long chatId)
        {
            _logger.Info("Processes /services command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Available services: " +
                $"{string.Join(", ", ConfigContract.AvailableBeautifiedServices)}."
            );
        }

        private async Task SendResponseToRequestCommand(long chatId)
        {
            _logger.Info("Processes /request command.");

            var jobParams = new StartJobParamsRequest();
            _userCache.TryAddUser(chatId, jobParams);

            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                ConfigContract.AvailableBeautifiedServices.ToArray(),
                new[] { "/cancel" },
            };

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Enter service name.",
                replyMarkup: replyKeyboard
            );
        }

        private async Task ContinueRequestCommandWithService(long chatId, string serviceName,
            StartJobParamsRequest requestParams)
        {
            string encodedUserInput = UserInputEncoder.Encode(serviceName);
            _logger.Info($"Continue process /request command with service {encodedUserInput}.");
            ReplyKeyboardMarkup replyKeyboard = new[]
            {
                ConfigContract.AvailableBeautifiedServices.ToArray(),
                new[] { "/cancel" },
            };

            if (!ConfigContract.ContainsService(serviceName))
            {
                await _botService.Client.SendTextMessageAsync(
                    chatId,
                    "Invalid service name. Please, try again.",
                    replyMarkup: replyKeyboard
                );
                return;
            }

            serviceName = ConfigContract.GetProperServiceName(serviceName);
            requestParams.Requirements = CreateRequirements(serviceName, $"{serviceName}Common");

            string message = _textProcessor.JoinWithNewLineSeparator(new[] {
                $"Enter data for {serviceName}. Please, use this format:",
                 "thingName1",
                 "thingName2"
            });

            await _botService.Client.SendTextMessageAsync(
                chatId,
                message,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task ContinueRequestCommandWithData(long chatId, IReadOnlyList<string> data,
            StartJobParamsRequest jobParams)
        {
            _logger.Info("Continue process /request command with data.");

            jobParams.ThingNames = data.ToList();

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Send request to process data. Return later to see results.",
                replyMarkup: new ReplyKeyboardRemove()
            );

            // Schedule task for request and waiting for service response.
            _ = ProcessingResponseReceiver.ScheduleRequestAsync(
                _botService, _serviceProxy, chatId, jobParams
            );

            _userCache.TryRemoveUser(chatId);
        }

        private async Task SendResponseToCancelCommand(long chatId)
        {
            _logger.Info("Processes /cancel command.");

            string message;
            if (_userCache.TryRemoveUser(chatId))
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

        private static ConfigRequirements CreateRequirements(string serviceName,
            string appraisalName)
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
