using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetChatById
{
    public class GetChatByIdQueryHandler : IRequestHandler<GetChatByIdQuery, ChatShortDTO>
    {
        private readonly MessengerDbContext _context;

        public GetChatByIdQueryHandler(MessengerDbContext context, ICacheService cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ChatShortDTO> Handle(GetChatByIdQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Chats
                   .AsNoTracking()
                   .Include(x=>x.Accounts)
                        .ThenInclude(_=>_.Account)
                   .Include(x=>x.Documents)
                   .FirstOrDefaultAsync(x => x.ChatId.Equals(request.ChatId), cancellationToken);

            return ChatShortDTO.Create(data);
        }
    }
}