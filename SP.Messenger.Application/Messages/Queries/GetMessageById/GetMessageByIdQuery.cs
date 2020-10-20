using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.Messages.Queries.GetMessageById
{
    public class GetMessageByIdQuery : IRequest<MessageClient>
    {
        public GetMessageByIdQuery(long messageId)
        {
            MessageId = messageId;
        }

        public long MessageId { get; }

        public static GetMessageByIdQuery Create(long messageId) 
            => new GetMessageByIdQuery(messageId);
    }
}
