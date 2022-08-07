using System;
using System.Threading.Tasks;
using Acolyte.Assertions;

namespace ProjectV.CommonWebApi.Service.Setup.Actions
{
    public sealed class FuncServiceSetupAction : IServiceSetupAction
    {
        private readonly Func<Task> _action;


        public FuncServiceSetupAction(
            Func<Task> action)
        {
            _action = action.ThrowIfNull(nameof(action));
        }

        #region IServiceSetupAction Implementation

        public async Task DoAsync()
        {
            await _action();
        }

        #endregion
    }
}
