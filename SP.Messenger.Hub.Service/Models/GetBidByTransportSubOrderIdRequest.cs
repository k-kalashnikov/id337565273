using System;

namespace SP.Consumers.Models
{
    public class GetBidByTransportSubOrderIdRequest
    {
        public GetBidByTransportSubOrderIdRequest(Guid transportSuborderId)
        {
            TransportSuborderId = transportSuborderId;
        }
        public Guid TransportSuborderId { get; }
        public static GetBidByTransportSubOrderIdRequest Create(Guid transportSuborderId)
            => new GetBidByTransportSubOrderIdRequest(transportSuborderId);
    }
}