using System;

namespace SP.Consumers.Models
{
    public class BidCommonResponse
    {
        public Guid DocumentId { get; set; }
        public long ChatId { get; set; }

        public static BidCommonResponse Create(Guid documentId, long chatId)
            => new BidCommonResponse
            {
                DocumentId = documentId,
                ChatId = chatId
            };
    }
}
