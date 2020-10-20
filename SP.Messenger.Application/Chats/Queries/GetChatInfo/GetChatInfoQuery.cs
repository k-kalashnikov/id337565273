using System;
using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatInfo
{
    public class GetChatInfoQuery : IRequest<ChatLightDTO>
    {
        public GetChatInfoQuery(long accountId, Guid documentId, long? contractorId)
        {
            AccountId = accountId;
            DocumentId = documentId;
            ContractorId = contractorId;
        }
        public long AccountId { get; }
        public Guid DocumentId { get; }
        public long? ContractorId { get; }

        public static GetChatInfoQuery Create(long accountId, Guid documentId, long? contractorId)
            => new GetChatInfoQuery(accountId, documentId, contractorId);
    }
}