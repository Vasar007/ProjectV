using System.Collections.Generic;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.CommonWebApi.Service.Setup.Actions;

namespace ProjectV.CommonWebApi.Service.Setup.Handlers
{
    public sealed class ServicePreRunHandler
    {
        public IReadOnlyList<IServiceSetupAction> Actions { get; }

        public IServiceSetupAction OnRunFailAction { get; }


        public ServicePreRunHandler(
            IReadOnlyList<IServiceSetupAction> actions,
            IServiceSetupAction onRunFailAction)
        {
            Actions = actions.ThrowIfNull(nameof(actions));
            OnRunFailAction = onRunFailAction.ThrowIfNull(nameof(onRunFailAction));
        }

        public static ServicePreRunHandler CreateWithPossibleNoOpOnRunAction(
            IReadOnlyList<IServiceSetupAction> actions,
            IServiceSetupAction? onRunFailAction)
        {
            if (onRunFailAction is null)
            {
                onRunFailAction = new FuncServiceSetupAction(() => Task.CompletedTask);
            }

            return new ServicePreRunHandler(
                actions: actions,
                onRunFailAction: onRunFailAction
            );
        }
    }
}
