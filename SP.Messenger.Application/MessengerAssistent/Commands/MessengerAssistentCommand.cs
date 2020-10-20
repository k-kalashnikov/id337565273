using MediatR;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.MessengerAssistent.Commands
{
    public class MessengerAssistentCommand : IRequest<ProcessingResult<bool>>
    {
        public MessengerAssistentCommand(MessengerServerEvent message)
        {
            Message = message;
        }
        public MessengerServerEvent Message { get; }
        
        public static MessengerAssistentCommand Create(MessengerServerEvent message)
            => new MessengerAssistentCommand(message);
    }
}