using MediatR;
using SP.Messenger.Application.Chats.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatType
{
    public class GetChatTypeIdQuery : IRequest<ChatTypeDTO>
    {
        public GetChatTypeIdQuery(int chatTypeId)
        {
            ChatTypeId = chatTypeId;
        }
        public int ChatTypeId { get; }

        public static GetChatTypeIdQuery Create(int chatTypeId)
            => new GetChatTypeIdQuery(chatTypeId);
    }
}
