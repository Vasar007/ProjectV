using System.Threading.Tasks;

namespace ProjectV.DesktopApp.Domain.Executor
{
    internal interface IPerformer<TInput, TResult>
    {
        Task<TResult> PerformAsync(TInput arg);
    }
}
