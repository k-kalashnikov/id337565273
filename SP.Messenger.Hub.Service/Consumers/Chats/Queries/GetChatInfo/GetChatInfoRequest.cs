using System;

namespace SP.Consumers.Models
{
    public class GetChatInfoRequest
    {
        public GetChatInfoRequest(long accountId, Guid documentId, long? contractorId)
        {
            AccountId = accountId;
            DocumentId = documentId;
            ContractorId = contractorId;
        }
        public long AccountId { get; set; }
        public Guid DocumentId { get; set; }
        public long? ContractorId { get; }

        public static GetChatInfoRequest Create(long accountId, Guid documentId, long? contractorId)
            => new GetChatInfoRequest(accountId, documentId, contractorId);
    }
}