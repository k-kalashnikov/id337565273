using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.MessageType.Queries
{
    public class GetMessagesByOrderQueryHandler : IRequestHandler<GetMessagesTypeQuery, MessageTypeDTO[]>
    {
        private readonly MessengerDbContext _context;
        public GetMessagesByOrderQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<MessageTypeDTO[]> Handle(GetMessagesTypeQuery request, CancellationToken cancellationToken)
        {
            var messageTypes = await _context.MessageTypes.AsNoTracking().ToArrayAsync(cancellationToken);
            return MessageTypeDTO.Create(messageTypes);
        }
    }
}
