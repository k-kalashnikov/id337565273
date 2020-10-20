using System;
using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatByDocumentStatus
{
    public class GetChatByDocumentStatusQuery : IRequest<ChatLightDTO>
    {
        public GetChatByDocumentStatusQuery(Guid documentId, string documentStatusMnemonic)
        {
            DocumentId = documentId;
            DocumentStatusMnemonic = documentStatusMnemonic;
        }
        public Guid DocumentId { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        
        public static GetChatByDocumentStatusQuery Create(Guid documentId, string documentStatusMnemonic)
            => new GetChatByDocumentStatusQuery(documentId, documentStatusMnemonic);
    }
}