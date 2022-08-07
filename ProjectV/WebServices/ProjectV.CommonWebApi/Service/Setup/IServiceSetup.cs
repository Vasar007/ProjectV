using System.Threading;
using System.Threading.Tasks;
using Acolyte.Common.Disposal;

namespace ProjectV.CommonWebApi.Service.Setup
{
    public interface IServiceSetup
    {
        Task<AsyncDisposableAction> PreRunAsync(CancellationToken cancellationToken = default);
        Task PostRunAsync(CancellationToken cancellationToken = default);
    }
}
