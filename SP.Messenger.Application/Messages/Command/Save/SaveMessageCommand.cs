using MediatR;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Messages.Command
{
    public class SaveMessageCommand : IRequest<ProcessingResult<long>>
    {
        public SaveMessageCommand(Message message, string userName)
        {
            Message = message;
            UserName = userName;
        }

        public Message Message { get; }
        public string UserName { get; }
        
        public static SaveMessageCommand Create(Message message, string userName)
            => new SaveMessageCommand(message, userName); 
        
    }
}
