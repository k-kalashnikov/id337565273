using MediatR;
using SP.Consumers.Models;


namespace SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds
{
    public class GetAccountsByOrganizationIdsQuery : IRequest<AccountMessengerDTO[]>
    {
        public GetAccountsByOrganizationIdsQuery(long[] organizationIds)
        {
            OrganizationIds = organizationIds;
        }
        public GetAccountsByOrganizationIdsQuery(long[] organizationIds, string chatTypeMnemonic)
        {
            OrganizationIds = organizationIds;
            ChatTypeMnemonic = chatTypeMnemonic;
        }
        public long[] OrganizationIds { get; }
        public string ChatTypeMnemonic { get; }

        public static GetAccountsByOrganizationIdsQuery Create(long[] organizationIds)
            => new GetAccountsByOrganizationIdsQuery(organizationIds);
        public static GetAccountsByOrganizationIdsQuery Create(long[] organizationIds, string chatTypeMnemonic)
           => new GetAccountsByOrganizationIdsQuery(organizationIds, chatTypeMnemonic);
    }
}