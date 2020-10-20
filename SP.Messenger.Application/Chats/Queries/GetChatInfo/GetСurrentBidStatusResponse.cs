using System;

namespace SP.Consumers.Models
{
    public class GetCurrentBidStatusResponse
    {
        public GetCurrentBidStatusResponse(Guid bidId, string bidStatusMnemonic, string bidStatusName, byte bidStatusId)
        {
            BidId = bidId;
            BidStatusMnemonic = bidStatusMnemonic;
            BidStatusName = bidStatusName;
            BidStatusId = bidStatusId;
        }

        public Guid BidId { get; set; }
        public string BidStatusMnemonic { get; set; }
        public string BidStatusName { get; set; }
        public byte BidStatusId { get; set; }
    }
}