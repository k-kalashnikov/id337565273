using MediatR;
using SP.Messenger.Application.Chats.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatType
{
    public class GetChatTypeMnemonicQuery : IRequest<ChatTypeDTO>
    {
        public GetChatTypeMnemonicQuery(string chatTypeMnemonic)
        {
            ChatTypeMnemonic = chatTypeMnemonic;
        }
        public string ChatTypeMnemonic { get; }

        public static GetChatTypeMnemonicQuery Create(string chatTypeMnemonic)
            => new GetChatTypeMnemonicQuery(chatTypeMnemonic);
    }
}
