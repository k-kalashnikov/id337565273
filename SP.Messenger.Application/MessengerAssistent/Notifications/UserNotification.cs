using MediatR;
using SP.Market.EventBus.RMQ.Shared.Events;

namespace SP.Messenger.Application.MessengerAssistent.Notifications
{
    public class UserNotification : INotification
    {
        public UserNotification(MessengerClientEvent @event)
        {
            @Event = @event;
        }
        public MessengerClientEvent @Event { get; }
        
        public static UserNotification Create(MessengerClientEvent @event)
            => new UserNotification(@event);
    }
}