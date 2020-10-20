using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatById
{
    public class GetChatByIdQuery : IRequest<ChatShortDTO>
    {
        public GetChatByIdQuery(long chatId)
        {
            ChatId = chatId;
        }
        public long ChatId { get; }
        
        public static GetChatByIdQuery Create(long chatId)
            => new GetChatByIdQuery(chatId);
    }
}