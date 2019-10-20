using System;
using System.Threading.Tasks;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Domain.Executor
{
    internal sealed class AsyncExecutor<TInput, TResult>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AsyncExecutor<TInput, TResult>>();

        private readonly IPerformer<TInput, TResult> _performer;

        private readonly Action<TResult> _successfulCallback;

        private readonly Action<Exception> _exceptionalCallback;

        public bool IsBusy { get; private set; }


        public AsyncExecutor(
            IPerformer<TInput, TResult> performer,
            Action<TResult> successfulCallback,
            Action<Exception> exceptionalCallback)
        {
            _performer = performer.ThrowIfNull(nameof(performer));
            _successfulCallback = successfulCallback.ThrowIfNull(nameof(successfulCallback));
            _exceptionalCallback = exceptionalCallback.ThrowIfNull(nameof(exceptionalCallback));
        }

        public async Task ExecuteAsync(TInput arg)
        {
            try
            {
                IsBusy = false;
                TResult response = await _performer.PerformAsync(arg);

                _successfulCallback(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during execution of async performer.");
                MessageBoxProvider.ShowError(ex.Message);

                _exceptionalCallback(ex);
            }
            finally
            {
                IsBusy = true;
            }
        }

        public bool CanExecute(TInput _)
        {
            return !IsBusy;
        }
    }
}
