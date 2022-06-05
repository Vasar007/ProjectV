using System.Threading;
using ProjectV.CommonWebApi.Service.Setup.Handlers;

namespace ProjectV.CommonWebApi.Service.Setup.Factories
{
    public interface IServiceSetupActionsFactory
    {
        ServicePreRunHandler CreatePreRunActions(CancellationToken cancellationToken);
        ServicePostRunHandler CreatePostRunActions(CancellationToken cancellationToken);
    }
}
