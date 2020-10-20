using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.Remove
{
    public class RemoveMessageCommand : IRequest<ProcessingResult<bool>>
    {
        public RemoveMessageCommand(long accountId, long messageId)
        {
            AccountId = accountId;
            MessageId = messageId;
        }

        public long AccountId { get; }
        public long MessageId { get; }
        
        public static RemoveMessageCommand Create(long accountId, long messageId)
            => new RemoveMessageCommand(accountId, messageId);
    }
}