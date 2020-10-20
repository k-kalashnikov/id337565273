using MediatR;
using SP.Consumers.Models;

namespace SP.Messenger.Application.MessageType.Queries
{
    public class GetMessagesTypeQuery : IRequest<MessageTypeDTO[]>
    {
    }
}
