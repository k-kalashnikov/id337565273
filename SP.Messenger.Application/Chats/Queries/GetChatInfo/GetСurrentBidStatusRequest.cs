using System;

namespace SP.Consumers.Models
{
    public class GetCurrentBidStatusRequest2
    {
        public GetCurrentBidStatusRequest2(long accountId, Guid bidId)
        {
            AccountId = accountId;
            BidId = bidId;
        }
        public long AccountId { get; set; }
        public Guid BidId { get; set; }
        
        public static GetCurrentBidStatusRequest2 Create(long accountId, Guid bidId)
            => new GetCurrentBidStatusRequest2(accountId, bidId);
    }
}