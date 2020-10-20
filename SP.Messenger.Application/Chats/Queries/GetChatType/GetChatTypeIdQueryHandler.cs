using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetChatType
{
    public class GetChatTypeIdQueryHandler : IRequestHandler<GetChatTypeIdQuery, ChatTypeDTO>
    {
        private readonly MessengerDbContext _context;
        private readonly ICacheService _cache;

        public GetChatTypeIdQueryHandler(MessengerDbContext context, ICacheService cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ChatTypeDTO> Handle(GetChatTypeIdQuery request, CancellationToken cancellationToken)
        {
            var key = request.ChatTypeId.ToString();

            var chatType = await _cache.GetAsync<ChatTypeDTO>(key, cancellationToken);
            if (chatType is null)
            {
                var chatDb = await _context.ChatTypes
                           .AsNoTracking()
                           .FirstOrDefaultAsync(x => x.ChatTypeId.Equals(request.ChatTypeId), cancellationToken);

                chatType = ChatTypeDTO.Create(chatDb);
                await _cache.SetAsync<ChatTypeDTO>(key, chatType, 6, 15, cancellationToken);
            }

            return chatType;
        }
    }
}
