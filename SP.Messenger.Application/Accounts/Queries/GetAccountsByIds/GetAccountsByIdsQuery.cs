using MediatR;
using SP.Consumers.Models;
using System.Collections.Generic;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsByIds
{
    public class GetAccountsByIdsQuery : IRequest<AccountMessengerDTO[]>
    {
        public GetAccountsByIdsQuery(IEnumerable<long> ids)
        {
            Ids = ids;
        }

        public IEnumerable<long> Ids { get; }

        public static GetAccountsByIdsQuery Create(IEnumerable<long> ids)
            => new GetAccountsByIdsQuery(ids);
    }
}
