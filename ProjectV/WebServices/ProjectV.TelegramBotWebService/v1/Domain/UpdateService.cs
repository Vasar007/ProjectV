#pragma warning disable format // dotnet format fails indentation for switch :(

using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.v1.Domain.Handlers;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
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

        public async Task HandleUpdateAsync(Update update,
            CancellationToken cancellationToken = default)
        {
            if (update is null)
            {
                _logger.Warn("Received empty update request.");
                return;
            }

            try
            {
                await HandleUpdateInternalAsync(update, cancellationToken);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex, cancellationToken);
            }
        }

        public Task HandleErrorAsync(Exception exception,
            CancellationToken cancellationToken = default)
        {
            _logger.Error(exception, $"Failed to process update request.");
            return Task.CompletedTask;
        }

        #endregion

        private async Task HandleUpdateInternalAsync(Update update,
            CancellationToken cancellationToken)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await ProcessIfNotNull(update.Message, _messageHandler.ProcessAsync, cancellationToken);
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

        private static async Task ProcessIfNotNull<TType>(TType? type, Func<TType, CancellationToken, Task> handle,
            CancellationToken cancellationToken)
        {
            if (type is null)
            {
                _logger.Warn("Message is empty, skipping processing");
                return;
            }

            await handle(type, cancellationToken);
        }
    }
}
