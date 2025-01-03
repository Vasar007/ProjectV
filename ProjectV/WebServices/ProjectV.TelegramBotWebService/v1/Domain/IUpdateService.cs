﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IUpdateService : IDisposable
    {
        Task HandleUpdateAsync(Update update, CancellationToken cancellationToken = default);
        Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken = default);
        Task HandleErrorAsync(Exception exception, HandleErrorSource source, CancellationToken cancellationToken = default);
    }
}
