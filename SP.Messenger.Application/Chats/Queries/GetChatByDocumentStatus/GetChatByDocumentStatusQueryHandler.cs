using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Queries.GetChatByDocumentStatus
{
    public class GetChatByDocumentStatusQueryHandler : IRequestHandler<GetChatByDocumentStatusQuery, ChatLightDTO>
    {
        private readonly MessengerDbContext _context;
        
        public GetChatByDocumentStatusQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<ChatLightDTO> Handle(GetChatByDocumentStatusQuery request, CancellationToken cancellationToken)
        {
            var chat = await _context.ChatView
                .Where(x => x.DocumentId == request.DocumentId.ToString()
                            && x.DocumentStatusMnemonic == request.DocumentStatusMnemonic)
                .FirstOrDefaultAsync(cancellationToken);
            
            return ChatLightDTO.Create(chat);
        }
    }
}