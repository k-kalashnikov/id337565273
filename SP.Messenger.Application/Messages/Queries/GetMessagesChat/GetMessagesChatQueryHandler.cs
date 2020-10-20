using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.FileStorage.Client.Services;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Messages.Queries.GetMessagesChat
{
  public class GetMessagesChatQueryHandler : IRequestHandler<GetMessagesChatQuery, MessageClient[]>
  {
    private readonly MessengerDbContext _context;
    private readonly IMediator _mediator;
    private readonly IFileStorageClientService _fileStorage;

    public GetMessagesChatQueryHandler(MessengerDbContext context,
        IMediator mediator,
        IFileStorageClientService fileStorage)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public async Task<MessageClient[]> Handle(GetMessagesChatQuery request, CancellationToken cancellationToken)
    {
      var union = _context.Messages
        .AsNoTracking()
        .Where(y => y.MessageTypeId.Equals(3) && y.RecipientId.Equals(request.AccountId) && y.ChatId == request.ChatId);

      var messages = await _context.Messages
                      .AsNoTracking()
                      .Where(x => x.ChatId == request.ChatId && x.MessageTypeId != 3)
                      .Union(union)
                      .OrderBy(x => x.CreateDate)
                      .Include(x => x.Account)
                      .Include(x => x.Chat)
                      .Include(x => x.MessageType)
                      .ToArrayAsync(cancellationToken);

      var serverInfo = await _fileStorage.GetServerInfoAsync(cancellationToken);
      return await MessageInsideDTO.CreateFromMessage(messages, serverInfo.Link, _mediator);
    }
  }
}
