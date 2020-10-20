using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsFromPlatform
{
    public class GetAccountsFromPlatformQuery : IRequest<AccountMessengerDTO[]>
    {
        public GetAccountsFromPlatformQuery(long? organizationId)
        {
            OrganizationId = organizationId;
        }
        public long? OrganizationId { get; }

        public static GetAccountsFromPlatformQuery Create(long? organizationId)
            => new GetAccountsFromPlatformQuery(organizationId);
    }
}
