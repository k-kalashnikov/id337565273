using MediatR;
using SP.Consumers.Models;
using System;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountByDocument
{
    public class GetAccountByDocumentQuery : IRequest<AccountMessengerDTO[]>
    {
        public GetAccountByDocumentQuery(Guid documentId)
        {
            DocumentId = documentId;
        }

        public Guid DocumentId { get; }

        public static GetAccountByDocumentQuery Create(Guid documentId)
            => new GetAccountByDocumentQuery(documentId);
    }
}
