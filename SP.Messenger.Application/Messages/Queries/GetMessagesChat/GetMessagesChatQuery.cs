using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Messages.Queries.GetMessagesChat
{
    public class GetMessagesChatQuery : IRequest<MessageClient[]>
    {
        public GetMessagesChatQuery(long accountId, long chatId)
        {
            AccountId = accountId;
            ChatId = chatId;
        }

        public long AccountId { get; }
        public long ChatId { get; }
        
        public static GetMessagesChatQuery Create(long accountId, long chatId)
            => new GetMessagesChatQuery(accountId, chatId);
    }
}