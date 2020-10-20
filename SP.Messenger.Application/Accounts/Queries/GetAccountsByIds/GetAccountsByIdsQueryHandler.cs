using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Infrastructure.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsByIds
{
    public class GetAccountsByIdsQueryHandler : IRequestHandler<GetAccountsByIdsQuery, AccountMessengerDTO[]>
    {
        private readonly IAccountPlatformService<GetAccountsByIdsRequest, GetAccountsByIdsResponse> _client;

        public GetAccountsByIdsQueryHandler(IAccountPlatformService<GetAccountsByIdsRequest, GetAccountsByIdsResponse> client)
        {
            _client = client;
        }

        public async Task<AccountMessengerDTO[]> Handle(GetAccountsByIdsQuery request, CancellationToken token)
        {
            var listIds = request.Ids.ToList();
            var requestIds = new GetAccountsByIdsRequest(listIds.ToArray());

            var result = await _client.GetAccount(requestIds, token);

            return AccountMessengerDTO.Create(result);
        }
    }


}
