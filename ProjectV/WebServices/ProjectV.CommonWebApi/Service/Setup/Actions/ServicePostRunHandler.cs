using System.Collections.Generic;
using Acolyte.Assertions;

namespace ProjectV.CommonWebApi.Service.Setup.Actions
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
