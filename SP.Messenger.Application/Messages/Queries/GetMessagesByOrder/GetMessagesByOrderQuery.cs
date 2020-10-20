using MediatR;
using SP.Messenger.Application.Messages.Models;
using System;

namespace SP.Messenger.Application.Messanges.Queries.GetMessagesByOrder
{
    public class GetMessagesByOrderQuery : IRequest<MessageDTO[]>
    {
        public GetMessagesByOrderQuery(Guid orderId)
        {
            OrderId = orderId;
        }

        public Guid OrderId { get; }
    }
}
