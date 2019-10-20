using System.Threading.Tasks;

namespace ThingAppraiser.DesktopApp.Domain.Executor
{
    internal interface IPerformer<TInput, TResult>
    {
        Task<TResult> PerformAsync(TInput arg);
    }
}
