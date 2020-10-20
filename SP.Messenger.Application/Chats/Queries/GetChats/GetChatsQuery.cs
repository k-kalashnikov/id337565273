using MediatR;
using SP.Messenger.Application.Chats.Models;
using System;

namespace SP.Messenger.Application.Chats.Queries.GetChats
{
    public class GetChatsQuery : IRequest<ChatMessengerDTO[]>
    {
        public long AccountId { get; set; }
        public Guid DocumentId { get; set; }

        public static GetChatsQuery Create(long accountId, Guid documentId)
            => new GetChatsQuery
            {
                AccountId = accountId,
                DocumentId = documentId,
            };
    }
}