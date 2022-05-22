using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Building;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.Properties;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Cache;
using ProjectV.TelegramBotWebService.v1.Domain.Receivers;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.v1.Domain.Handlers
{
    public sealed class BotMessageHandler : IBotHandler<Message>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<BotMessageHandler>();

        private readonly IBotService _botService;
        private readonly IProcessingResponseReceiver _responseReceiver;
        private readonly IUserCache _userCache;
        private readonly ITelegramTextProcessor _textProcessor;

        private readonly TelegramBotWebServiceOptions _options;

        private string StartCommand => _options.Bot.Commands.StartCommand;
        private string ServicesCommand => _options.Bot.Commands.ServicesCommand;
        private string RequestCommand => _options.Bot.Commands.RequestCommand;
        private string CancelCommand => _options.Bot.Commands.CancelCommand;
        private string HelpCommand => _options.Bot.Commands.HelpCommand;


        public BotMessageHandler(
            IBotService botService,
            IProcessingResponseReceiver responseReceiver,
            IUserCache userCache,
            ITelegramTextProcessor textProcessor,
            IOptions<TelegramBotWebServiceOptions> options)
        {
            _botService = botService.ThrowIfNull(nameof(botService));
            _responseReceiver = responseReceiver.ThrowIfNull(nameof(responseReceiver));
            _userCache = userCache.ThrowIfNull(nameof(userCache));
            _textProcessor = textProcessor.ThrowIfNull(nameof(textProcessor));
            _options = options.GetCheckedValue();
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _responseReceiver.Dispose();

            _disposed = true;
        }

        #endregion

        #region IBotCommandHandler Implementation

        public async Task ProcessAsync(Message message)
        {
            _logger.Info($"Received Message from {message.Chat.Id}.");

            if (message.Text is null)
            {
                _logger.Warn("Message does not contain text.");
                return;
            }

            IReadOnlyList<string> data = _textProcessor.ParseAsSeparateLines(message.Text);

            string command = _textProcessor.ParseCommand(data[0]);

            Task responseTask = command switch
            {
                _ when IsCommand(command, StartCommand) => SendResponseToStartCommand(message.Chat.Id),

                _ when IsCommand(command, ServicesCommand) => SendResponseToServicesCommand(message.Chat.Id),

                _ when IsCommand(command, RequestCommand) => SendResponseToRequestCommand(message.Chat.Id),

                _ when IsCommand(command, CancelCommand) => SendResponseToCancelCommand(message.Chat.Id),

                _ when IsCommand(command, HelpCommand) => SendResponseToHelpCommand(message.Chat.Id),

                _ => TryHandleContinuation(message, data)
            };

            await responseTask;
        }

        #endregion

        private static bool IsCommand(string expectedCommand, string actualCommand)
        {
            return StringComparer.Ordinal.Equals(expectedCommand, actualCommand);
        }

        private async Task SendResponseToStartCommand(long chatId)
        {
            _logger.Info($"Processes {StartCommand} command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                Messages.HelloMessage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToHelpCommand(long chatId)
        {
            _logger.Info($"Processes {HelpCommand} command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                Messages.HelpMessage,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task SendResponseToServicesCommand(long chatId)
        {
            _logger.Info($"Processes {ServicesCommand} command.");

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Available services: " +
                $"{string.Join(", ", ConfigContract.AvailableBeautifiedServices)}."
            );
        }

        private async Task SendResponseToRequestCommand(long chatId)
        {
            _logger.Info($"Processes {RequestCommand} command.");

            var jobParams = new StartJobParamsRequest();
            _userCache.TryAddUser(chatId, jobParams);

            ReplyKeyboardMarkup? replyKeyboard = new[]
            {
                ConfigContract.AvailableBeautifiedServices.ToArray(),
                new[] { CancelCommand }
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
            string userInput = _textProcessor.TrimNewLineSeparator(serviceName);
            _logger.Info($"Continue process {RequestCommand} command with service {userInput}.");

            ReplyKeyboardMarkup? replyKeyboard = new[]
            {
                ConfigContract.AvailableBeautifiedServices.ToArray(),
                new[] { CancelCommand }
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

            string message = _textProcessor.JoinWithNewLineLSeparator(new[]
            {
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
            _logger.Info($"Continue process {RequestCommand} command with data.");

            jobParams.ThingNames = data.ToList();

            await _botService.Client.SendTextMessageAsync(
                chatId,
                "Send request to process data. Return later to see results.",
                replyMarkup: new ReplyKeyboardRemove()
            );

            // Schedule task for request and waiting for service response.
            _ = _responseReceiver.ScheduleRequestAsync(_botService, chatId, jobParams);

            _userCache.TryRemoveUser(chatId);
        }

        private async Task SendResponseToCancelCommand(long chatId)
        {
            _logger.Info($"Processes {CancelCommand} command.");

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
                $"Invalid message. See usage at {HelpCommand} command.",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        private async Task TryHandleContinuation(Message message, IReadOnlyList<string> data)
        {
            if (!_userCache.TryGetUser(message.Chat.Id, out StartJobParamsRequest? jobData))
            {
                await SendResponseToInvalidMessage(message.Chat.Id);
                return;
            }

            if (jobData.Requirements is null)
            {
                string serviceName = data[0];
                await ContinueRequestCommandWithService(
                    message.Chat.Id, serviceName, jobData
                );
                return;
            }

            await ContinueRequestCommandWithData(message.Chat.Id, data, jobData);
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
