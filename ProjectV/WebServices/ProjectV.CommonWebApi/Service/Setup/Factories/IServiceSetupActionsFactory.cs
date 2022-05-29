using ProjectV.CommonWebApi.Service.Setup.Actions;

namespace ProjectV.CommonWebApi.Service.Setup.Factories
{
    public interface IServiceSetupActionsFactory
    {
        ServicePreRunHandler CreatePreRunActions();
        ServicePostRunHandler CreatePostRunActions();
    }
}
