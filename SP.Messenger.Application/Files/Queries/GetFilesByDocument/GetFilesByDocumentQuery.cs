using System;
using MediatR;
using SP.Messenger.Application.Files.Models;

namespace SP.Messenger.Application.Files.Queries.GetFilesByDocument
{
    public class GetFilesByDocumentQuery : IRequest<FileShortDto[]>
    {
        public GetFilesByDocumentQuery(Guid documentId)
        {
            DocumentId = documentId;
        }
        public Guid DocumentId { get; }
        
        public static GetFilesByDocumentQuery Create(Guid documentId)
            =>new GetFilesByDocumentQuery(documentId);
    }
}