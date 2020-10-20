using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetChatByDocumentId
{
    public class GetChatByDocumentIdQueryHandler : IRequestHandler<GetChatByDocumentIdQuery, ChatMessengerDTO>
    {
        private readonly MessengerDbContext _context;
        public GetChatByDocumentIdQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ChatMessengerDTO> Handle(GetChatByDocumentIdQuery request, CancellationToken cancellationToken)
        {
            var chat = await _context.SpecificationView
                            .AsNoTracking()
                            .Include(x => x.ChatType)
                .FirstOrDefaultAsync(x => x.DocumentId == request.DocumentId, cancellationToken);

            return ChatMessengerDTO.Create(chat);
        }
    }
}
