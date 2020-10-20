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
    public class GetChatTypeByChatQueryHandler : IRequestHandler<GetChatTypeByChatQuery, ChatTypeDTO>
    {
        private readonly MessengerDbContext _context;
        private readonly ICacheService _cache;
        public GetChatTypeByChatQueryHandler(MessengerDbContext context, ICacheService cache)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ChatTypeDTO> Handle(GetChatTypeByChatQuery request, CancellationToken cancellationToken)
        {
            var key = request.ChatId.ToString();
            ChatTypeDTO chatType = await _cache.GetAsync<ChatTypeDTO>(key, cancellationToken);

            if (chatType is null)
            {
                var chatDb = await _context.Chats
                                .AsNoTracking()
                                .Include(x => x.ChatType)
                    .FirstOrDefaultAsync(x => x.ChatId.Equals(request.ChatId), cancellationToken);

                chatType = ChatTypeDTO.Create(chatDb?.ChatType);
                await _cache.SetAsync<ChatTypeDTO>(key, chatType, 6, 15, cancellationToken);
            }

            return chatType;
        }
    }
}
