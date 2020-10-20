using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SP.Messenger.Application.Messages.Models;

namespace SP.Messenger.Application.Messanges.Queries.GetMessagesByOrder
{
    public class GetMessagesByOrderQueryHandler : IRequestHandler<GetMessagesByOrderQuery, MessageDTO[]>
    {
        private readonly MessengerDbContext _context;
        public GetMessagesByOrderQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<MessageDTO[]> Handle(GetMessagesByOrderQuery request, CancellationToken cancellationToken)
        {
//            var messages = from chat in _context.Chats.AsNoTracking().Where(x => x.Name == request.OrderId.ToString())
//                            join message in _context.Messages on chat.ChatId equals message.ChatId into msg
//                            from message in msg.DefaultIfEmpty()
//                            select message;

            var messages = await (from chat in _context
                                    .Chats.AsNoTracking()
                                    .Where(x => x.Name == request.OrderId.ToString())
                    join message in _context.Messages on chat.ChatId equals message.ChatId into msg
                    from message in msg.DefaultIfEmpty()
                    select message
                    ).AsNoTracking()
                            .Include(x => x.Account)
                            .Include(x => x.Chat)
                            .Include(x => x.MessageType)
                            .ToArrayAsync(cancellationToken);

            return MessageDTO.Create(messages);
        }
    }
}
