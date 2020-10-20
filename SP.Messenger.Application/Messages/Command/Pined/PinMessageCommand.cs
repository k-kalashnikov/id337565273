using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.Pined
{
    public class PinMessageCommand : IRequest<ProcessingResult<bool>>
    {
        public PinMessageCommand(long messageId)
        {
            MessageId = messageId;
        }
        public long MessageId { get; }
        
        public static PinMessageCommand Create(long messageId)
            => new PinMessageCommand(messageId);
    }
}