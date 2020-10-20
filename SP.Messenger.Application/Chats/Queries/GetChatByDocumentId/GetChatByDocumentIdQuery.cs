using MediatR;
using SP.Messenger.Application.Chats.Models;
using System;

namespace SP.Messenger.Application.Chats.Queries.GetChatByDocumentId
{
    public class GetChatByDocumentIdQuery : IRequest<ChatMessengerDTO>
    {
        public GetChatByDocumentIdQuery(Guid documentId)
        {
            DocumentId = documentId;
        }
        public Guid DocumentId { get; }
    }
}
