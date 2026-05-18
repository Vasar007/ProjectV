using System;
using System.Threading.Tasks;

namespace ProjectV.CommonWebApi.Service.Setup.Actions
{
    public sealed class FuncServiceSetupActionFactory
    {
        public FuncServiceSetupActionFactory()
        {
        }

        public IServiceSetupAction Create(Func<Task> action, bool useSafeAction)
        {
            if (useSafeAction)
            {
                return new FuncServiceSetupActionSafe(action);
            }

            return new FuncServiceSetupAction(action);
        }
    }
}
