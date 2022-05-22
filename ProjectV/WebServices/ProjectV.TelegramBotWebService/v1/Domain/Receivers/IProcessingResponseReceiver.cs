using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectV.Models.WebServices.Requests;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Receivers
{
    public interface IProcessingResponseReceiver : IDisposable
    {
        Task ScheduleRequestAsync(IBotService botService, long chatId,
            StartJobParamsRequest jobParams, CancellationToken cancellationToken = default);
    }
}
