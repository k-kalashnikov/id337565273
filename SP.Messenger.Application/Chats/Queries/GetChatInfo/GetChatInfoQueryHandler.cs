using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Queries.GetChatInfo
{
    public class GetChatInfoQueryHandler : IRequestHandler<GetChatInfoQuery, ChatLightDTO>
    {
        private readonly MessengerDbContext _context;
        public GetChatInfoQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<ChatLightDTO> Handle(GetChatInfoQuery request, CancellationToken cancellationToken)
        {
            //x => x.DocumentId == request.DocumentId.ToString()
            //Используется при отправке файлов
            var chat = await _context.ChatView
                .Where(x => x.ParentDocumentId == request.DocumentId.ToString()
                            && x.ContractorId == request.ContractorId)
                .FirstOrDefaultAsync(cancellationToken);
            
            return ChatLightDTO.Create(chat);
        }
    }
}