using System;
using System.Collections.Generic;
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

        private readonly Lazy<ServicePreRunHandler> _preRunLazy;
        private ServicePreRunHandler PreRun => _preRunLazy.Value;

        private readonly Lazy<ServicePostRunHandler> _postRunLazy;
        private ServicePostRunHandler PostRun => _postRunLazy.Value;


        public ServiceSetup(
            IServiceSetupActionsFactory actionsFactory)
        {
            _actionsFactory = actionsFactory.ThrowIfNull(nameof(actionsFactory));

            _preRunLazy = new Lazy<ServicePreRunHandler>(
                () => _actionsFactory.CreatePreRunActions()
            );
            _postRunLazy = new Lazy<ServicePostRunHandler>(
                () => _actionsFactory.CreatePostRunActions()
            );
        }

        #region IServiceSetup Implementation

        public async Task<AsyncDisposableAction> PreRunAsync()
        {
            _logger.Info("Executing pre-run actions.");

            await ProcessActionsAsync(PreRun.Actions, "Failed to perform pre-run action.");

            _logger.Info("Pre-run actions have been completed.");
            return new AsyncDisposableAction(() => PreRun.OnRunFailAction.DoAsync());
        }

        public async Task PostRunAsync()
        {
            _logger.Info("Executing post-run actions.");

            await ProcessActionsAsync(PostRun.Actions, "Failed to perform post-run action.");

            _logger.Info("Post-run actions have been completed.");
        }

        #endregion

        private static async Task ProcessActionsAsync(IReadOnlyList<IServiceSetupAction> actions,
            string errorMessage)
        {
            var resultsOrExceptions = await actions
                .SafeParallelForEachAwaitAsync(action => action.DoAsync());

            var exceptions = resultsOrExceptions.UnwrapResultsOrExceptions();
            _logger.Errors(exceptions, errorMessage);
        }
    }
}
