using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Accounts
{
    public interface IAccountsOrganizationService<TRequest, TResponse>
    {
        Task<TResponse> GetAccountByOrganizationIds(TRequest request, CancellationToken cancellationToken);
    }
}