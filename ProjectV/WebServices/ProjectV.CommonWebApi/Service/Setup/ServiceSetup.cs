using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Disposal;
using Acolyte.Linq;
using Acolyte.Threading.Tasks;
using ProjectV.CommonWebApi.Service.Setup.Actions;
using ProjectV.CommonWebApi.Service.Setup.Factories;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi.Service.Setup
{
    public sealed class ServiceSetup : IServiceSetup
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceSetup>();

        private readonly IServiceSetupActionsFactory _actionsFactory;


        public ServiceSetup(
            IServiceSetupActionsFactory actionsFactory)
        {
            _actionsFactory = actionsFactory.ThrowIfNull(nameof(actionsFactory));
        }

        #region IServiceSetup Implementation

        public async Task<AsyncDisposableAction> PreRunAsync(
            CancellationToken cancellationToken = default)
        {
            _logger.Info("Executing pre-run actions.");

            var preRunHanlder = _actionsFactory.CreatePreRunActions(cancellationToken);
            await ProcessActionsAsync(
                preRunHanlder.Actions,
                "Failed to perform pre-run action.",
                cancellationToken
            );

            _logger.Info("Pre-run actions have been completed.");
            return new AsyncDisposableAction(() => preRunHanlder.OnRunFailAction.DoAsync());
        }

        public async Task PostRunAsync(CancellationToken cancellationToken = default)
        {
            _logger.Info("Executing post-run actions.");

            var postRunHanlder = _actionsFactory.CreatePostRunActions(cancellationToken);
            await ProcessActionsAsync(
                postRunHanlder.Actions,
                "Failed to perform post-run action.",
                cancellationToken
            );

            _logger.Info("Post-run actions have been completed.");
        }

        #endregion

        private static async Task ProcessActionsAsync(IReadOnlyList<IServiceSetupAction> actions,
            string errorMessage, CancellationToken cancellationToken)
        {
            var resultsOrExceptions = await actions
                .SafeParallelForEachAwaitAsync(action => action.DoAsync(), cancellationToken);

            var exceptions = resultsOrExceptions.UnwrapResultsOrExceptions();
            _logger.Errors(exceptions, errorMessage);
        }
    }
}
