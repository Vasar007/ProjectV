using System.Threading;
using ProjectV.CommonWebApi.Service.Setup.Actions;

namespace ProjectV.CommonWebApi.Service.Setup.Factories
{
    public interface IServiceSetupActionsFactory
    {
        ServicePreRunHandler CreatePreRunActions(CancellationToken cancellationToken = default);
        ServicePostRunHandler CreatePostRunActions(CancellationToken cancellationToken = default);
    }
}
