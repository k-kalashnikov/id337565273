using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace SP.Messenger.Application.MessengerAssistent.Notifications
{
    public class UserNotificationHandler : INotificationHandler<UserNotification>
    {
        private readonly IBusControl _bus;
        
        public UserNotificationHandler(IBusControl bus)
        {
            _bus = bus;
        }
        
        public async Task Handle(UserNotification notification, CancellationToken cancellationToken)
        {
            await _bus.Publish(notification.Event, cancellationToken);
        }
    }
}