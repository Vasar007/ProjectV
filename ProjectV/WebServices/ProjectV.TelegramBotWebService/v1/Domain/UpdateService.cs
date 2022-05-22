#pragma warning disable format // dotnet format fails indentation for switch :(

using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.v1.Domain.Handlers;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public sealed class UpdateService : IUpdateService
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<UpdateService>();

        private readonly ITelegramTextProcessor _textProcessor;
        private readonly IBotHandler<Message> _messageHandler;


        public UpdateService(
            ITelegramTextProcessor textProcessor,
            IBotHandler<Message> messageHandler)
        {
            _textProcessor = textProcessor.ThrowIfNull(nameof(textProcessor));
            _messageHandler = messageHandler.ThrowIfNull(nameof(messageHandler));
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _messageHandler.Dispose();

            _disposed = true;
        }

        #endregion

        #region IUpdateService Implementation

        public async Task ProcessUpdateRequestAsync(Update update)
        {
            if (update is null)
            {
                _logger.Warn("Received empty update request.");
                return;
            }

            try
            {
                await ProcessUpdateMessageInternalAsync(update);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        #endregion

        private async Task ProcessUpdateMessageInternalAsync(Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await ProcessIfNotNull(update.Message, _messageHandler.ProcessAsync);
                    break;
                }

                default:
                {
                    string message = _textProcessor.TrimNewLineSeparator($"Skipped {update.Type}.");
                    _logger.Warn(message);
                    break;
                }
            }
        }

        private static async Task ProcessIfNotNull<TType>(TType? type, Func<TType, Task> handle)
        {
            if (type is null)
            {
                _logger.Warn("Message is empty, skipping processing");
                return;
            }

            await handle(type);
        }

        private static Task HandleErrorAsync(Exception ex)
        {
            var errorMessage = ex switch
            {
                ApiRequestException apiRequestException =>
                    $"Telegram API Error [{apiRequestException.ErrorCode}]: " +
                    $"{apiRequestException.Message}",

                _ => ex.Message
            };

            _logger.Error(ex, $"Failed to process update request: {errorMessage}");
            return Task.CompletedTask;
        }
    }
}
