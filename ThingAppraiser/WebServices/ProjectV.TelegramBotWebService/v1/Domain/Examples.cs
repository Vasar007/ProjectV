using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ProjectV.Logging;

namespace ProjectV.TelegramBotWebService.v1.Domain.Examples
{
    // TODO: remove this class later!
    public sealed class Examples
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<Examples>();

        private readonly IBotService _botService;


        public Examples(IBotService botService)
        {
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        public async Task ProcessUpdateMessage(Update update)
        {
            if (update is null)
            {
                _logger.Warn("Received empty Message.");
                return;
            }
            if (update.Type != UpdateType.Message)
            {
                _logger.Warn("Received not Message.");
                return;
            }

            var message = update.Message;

            _logger.Info($"Received Message from {message.Chat.Id}.");

            await ProcessMessageWithCommands(message);
            await ProcessMessageSimple(message);
        }

        private async Task ProcessMessageSimple(Message message)
        {
            if (message.Type == MessageType.Text)
            {
                await _botService.Client.SendTextMessageAsync(message.Chat.Id, message.Text);
            }
            else if (message.Type == MessageType.Photo)
            {
                // Download Photo
                var fileId = message.Photo.LastOrDefault()?.FileId;
                var file = await _botService.Client.GetFileAsync(fileId);

                var filename = file.FileId + "." + file.FilePath.Split('.').Last();

                using (var saveImageStream = System.IO.File.Open(filename, FileMode.Create))
                {
                    await _botService.Client.DownloadFileAsync(file.FilePath, saveImageStream);
                }

                await _botService.Client.SendTextMessageAsync(message.Chat.Id, "Thx for the Pics");
            }
        }

        private async Task ProcessMessageWithCommands(Message message)
        {
            switch (message.Text.Split(' ').First())
            {
                // send inline keyboard
                case "/inline":
                    await _botService.Client.SendChatActionAsync(message.Chat.Id,
                                                                 ChatAction.Typing);

                    await Task.Delay(500); // simulate longer running task

                    var inlineKeyboard = new InlineKeyboardMarkup(new[]
                    {
                        new [] // first row
                        {
                            InlineKeyboardButton.WithCallbackData("1.1"),
                            InlineKeyboardButton.WithCallbackData("1.2"),
                        },
                        new [] // second row
                        {
                            InlineKeyboardButton.WithCallbackData("2.1"),
                            InlineKeyboardButton.WithCallbackData("2.2"),
                        }
                    });

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: inlineKeyboard
                    );
                    break;

                // send custom keyboard
                case "/keyboard":
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "1.1", "1.2" },
                        new[] { "2.1", "2.2" },
                    };

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Choose",
                        replyMarkup: ReplyKeyboard
                    );
                    break;

                // send a photo
                case "/photo":
                    await _botService.Client.SendChatActionAsync(message.Chat.Id,
                                                                 ChatAction.UploadPhoto);

                    const string file = @"Files/flat.jpg";

                    var fileName = file.Split(Path.DirectorySeparatorChar).Last();

                    using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                                                           FileShare.Read))
                    {
                        await _botService.Client.SendPhotoAsync(
                            message.Chat.Id,
                            fileStream,
                            "Nice Picture"
                        );
                    }
                    break;

                // request location or contact
                case "/request":
                    var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                    {
                        KeyboardButton.WithRequestLocation("Location"),
                        KeyboardButton.WithRequestContact("Contact"),
                    });

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        "Who or Where are you?",
                        replyMarkup: RequestReplyKeyboard
                    );
                    break;

                default:
                    const string usage = @"
Usage:
/inline   - send inline keyboard
/keyboard - send custom keyboard
/photo    - send a photo
/request  - request location or contact";

                    await _botService.Client.SendTextMessageAsync(
                        message.Chat.Id,
                        usage,
                        replyMarkup: new ReplyKeyboardRemove());
                    break;
            }
        }
    }
}
