using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Report
{
    public interface IContragentsProject<TRequest, TResponse>
    {
        Task<TResponse> GetContrgaentsByProject(TRequest request, CancellationToken cancellationToken);
    }
}