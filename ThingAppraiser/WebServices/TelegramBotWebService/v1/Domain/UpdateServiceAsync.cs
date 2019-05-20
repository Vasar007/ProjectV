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

                case "/input":
                {
                    string serviceName = firstLine[1];
                    await SendResponseToInputCommand(message.Chat.Id, serviceName, data);
                    break;
                }

                case "/help":
                {
                    await SendResponseToHelpCommand(message.Chat.Id);
                    break;
                }

                default:
                {
                    _logger.Info($"Skipped message: {message.Text}");
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
/input <service-name> — enter data for a request;
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

        private async Task SendResponseToInputCommand(long chatId, string serviceName,
            string[] data)
        {
            _logger.Info("Processes /input command.");

            var requestParams = new RequestParams
            {
                Requirements = CreateRequirements(serviceName, $"{serviceName}Common"),
                ThingNames = data.Skip(1).ToList()
            };
            _cache.TryAdd(chatId, requestParams);

            await _botService.Client.SendTextMessageAsync(
                chatId,
                $"Send request to process data for {serviceName}."
            );

            ProcessingResponseReceiver.ScheduleRequest(_botService, _serviceProxy,
                                                       requestParams, chatId);
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
