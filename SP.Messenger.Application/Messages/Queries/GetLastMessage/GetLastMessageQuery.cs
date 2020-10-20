using MediatR;
using SP.Messenger.Application.Chats.Models;
using System;

namespace SP.Messenger.Application.Chats.Queries.GetLastMessage
{
    public class GetLastMessageQuery : IRequest<ChatMessengerDTO[]>
    {
        public GetLastMessageQuery(Guid stockOrderId)
        {
            StockOrderId = stockOrderId;
        }
        public Guid StockOrderId { get; }

        public static GetLastMessageQuery Create(Guid stockOrderId)
        => new GetLastMessageQuery(stockOrderId);
    }
}
