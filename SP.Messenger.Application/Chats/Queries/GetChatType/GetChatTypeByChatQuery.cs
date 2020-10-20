using MediatR;
using SP.Messenger.Application.Chats.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatType
{
    public class GetChatTypeByChatQuery : IRequest<ChatTypeDTO>
    {
        public GetChatTypeByChatQuery(long chatId)
        {
            ChatId = chatId;
        }

        public long ChatId { get; set; }

        public static GetChatTypeByChatQuery Create(long chatId)
            => new GetChatTypeByChatQuery(chatId);
    }
}
