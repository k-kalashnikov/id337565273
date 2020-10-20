using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.CloseBotMessage
{
    public class CloseBotMessageCommand : IRequest<ProcessingResult<bool>>
    {
        public CloseBotMessageCommand(long messageId, dynamic value, string content)
        {
            MessageId = messageId;
            Value = value;
            Content = content;
        }

        public long MessageId { get; }
        public dynamic Value { get; }
        public string Content { get; }

        public static CloseBotMessageCommand Create(long messageId, dynamic value, string content)
            => new CloseBotMessageCommand(messageId, value, content);
    }
}