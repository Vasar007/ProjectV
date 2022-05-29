using System.Threading.Tasks;
using Acolyte.Common.Disposal;

namespace ProjectV.CommonWebApi.Service.Setup
{
    public interface IServiceSetup
    {
        Task<AsyncDisposableAction> PreRunAsync();
        Task PostRunAsync();
    }
}
