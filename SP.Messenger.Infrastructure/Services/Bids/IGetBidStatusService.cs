using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Bids
{
    public interface IGetBidStatusService<TRequest, TResponse>
    {
        Task<TResponse> GetBidStatus(TRequest request, CancellationToken cancellationToken);
    }
}