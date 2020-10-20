using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SP.FileStorage.Client.Services;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Common.Settings;
using SP.Messenger.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetLastMessage
{
    public class GetLastMessageQueryHandler : IRequestHandler<GetLastMessageQuery, ChatMessengerDTO[]>
    {
        private readonly MessengerDbContext _context;
        private readonly IOptions<Settings> _options;
        private readonly IFileStorageClientService _fileStorage;
        public GetLastMessageQueryHandler(MessengerDbContext context,
            IOptions<Settings> options,
            IFileStorageClientService fileStorage)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(context));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        public async Task<ChatMessengerDTO[]> Handle(GetLastMessageQuery request, CancellationToken cancellationToken)
        {
            var chats = await _context.LastMessagesView
                                .AsNoTracking()
                                .Where(x => x.DocumentId.Equals(request.StockOrderId))
                                .Include(x => x.Account)
                                .Include(x => x.ChatType)
                                .ToListAsync(cancellationToken);

            var serverInfo = await _fileStorage.GetServerInfoAsync(cancellationToken);
            return ChatMessengerDTO.Create(chats, serverInfo.Link);
        }
    }
}
