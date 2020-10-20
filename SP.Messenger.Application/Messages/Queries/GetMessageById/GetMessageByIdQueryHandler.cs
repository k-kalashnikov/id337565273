using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.FileStorage.Client.Services;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Messages.Queries.GetMessageById
{
    public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, MessageClient>
    {
        private readonly MessengerDbContext _context;
        private readonly IMediator _mediator;
        private readonly IFileStorageClientService _fileStorage;

        public GetMessageByIdQueryHandler(MessengerDbContext context, IMediator mediator, IFileStorageClientService fileStorage)
        {
            _context = context;
            _mediator = mediator;
            _fileStorage = fileStorage;
        }

        public async Task<MessageClient> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                .Include(x => x.MessageType)
                .Include(x=>x.Account)
                .Include(x => x.Chat)
                .FirstOrDefaultAsync(x=>x.MessageId == request.MessageId, cancellationToken);

            var serverInfo = await _fileStorage.GetServerInfoAsync(cancellationToken);
           
            return await MessageInsideDTO.CreateFromMessage(message, serverInfo.Link, _mediator);
        }
    }
}
