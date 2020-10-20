using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsChat
{
    public class GetAccountByChat : IRequest<AccountMessengerDTO[]>
    {
        public GetAccountByChat(long chatId, long? organizationId)
        {
            ChatId = chatId;
            OrganizationId = organizationId;
        }
        public long ChatId { get; }
        public long? OrganizationId { get; }

        public static GetAccountByChat Create(long chatId, long? organizationId)
            => new GetAccountByChat(chatId, organizationId);
    }
}
