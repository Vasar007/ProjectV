using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.CommonWebApi.Service.Setup.Actions;

namespace ProjectV.CommonWebApi.Service.Setup.Handlers
{
    public sealed class ServicePostRunHandler
    {
        public IReadOnlyList<IServiceSetupAction> Actions { get; }


        public ServicePostRunHandler(
            IReadOnlyList<IServiceSetupAction> actions)
        {
            Actions = actions.ThrowIfNull(nameof(actions));
        }
    }
}
