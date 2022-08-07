using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi.Service.Setup.Actions
{
    public sealed class FuncServiceSetupActionSafe : IServiceSetupAction
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<FuncServiceSetupActionSafe>();

        private readonly Func<Task> _action;


        public FuncServiceSetupActionSafe(
            Func<Task> action)
        {
            _action = action.ThrowIfNull(nameof(action));
        }

        #region IServiceSetupAction Implementation

        public async Task DoAsync()
        {
            try
            {
                await _action();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to perform service setup action.");
            }
        }

        #endregion
    }
}
