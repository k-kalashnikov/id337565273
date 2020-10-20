using MediatR;
using SP.Messenger.Application.ChatTypeRoles.Models;

namespace SP.Messenger.Application.ChatTypeRoles.Queries
{
    public class GetRolesQuery : IRequest<ChatTypeRolesDto>
    {
        public GetRolesQuery(string chatTypeMnemonic)
        {
            ChatTypeMnemonic = chatTypeMnemonic;
        }

        public string ChatTypeMnemonic { get; }

        public static GetRolesQuery Create(string chatTypeMnemonic)
            => new GetRolesQuery(chatTypeMnemonic);
    }
}
