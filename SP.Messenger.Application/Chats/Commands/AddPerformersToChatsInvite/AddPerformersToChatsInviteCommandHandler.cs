using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SP.Messenger.Application.Exceptions;

namespace SP.Messenger.Application.Chats.Commands.AddPerformersToChatsInvite
{
  public class AddPerformersToChatsInviteCommandHandler : IRequestHandler<AddPerformersToChatsInviteCommand, bool>
  {
    private readonly MessengerDbContext _context;

    public AddPerformersToChatsInviteCommandHandler(MessengerDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Handle(AddPerformersToChatsInviteCommand request, CancellationToken cancellationToken)
    {
      Log.Information($"{nameof(AddPerformersToChatsInviteCommandHandler)}");
      Log.Information($"get chatBidView by DocumentInviteId: {request.DocumentInviteId}");

      var chatBidView = await _context.ChatView
          .FirstOrDefaultAsync(x => x.DocumentId == request.DocumentInviteId.ToString(), cancellationToken);
      if (chatBidView is null)
        throw new NotFoundException(nameof(chatBidView), request.DocumentInviteId.ToString());

      var chatBid = await _context.Chats
          .Include(x => x.Accounts)
          .FirstOrDefaultAsync(x => x.ChatId == chatBidView.ChatId, cancellationToken);
      if (chatBid is null)
        throw new NotFoundException(nameof(chatBid), chatBidView.ChatId.ToString());

      var chatPurchase = await _context.Chats
          .Include(x => x.Accounts)
          .FirstOrDefaultAsync(x => x.ChatId == chatBid.ParentId, cancellationToken);

      Log.Information($"{nameof(chatPurchase)} NotFoundException: { chatBid.ParentId.ToString()}");

      foreach (var item in request.Accounts)
      {
        Log.Information($"{nameof(AddPerformersToChatsInviteCommandHandler)} AccountId =  {item.AccountId}");
        Log.Information($"{nameof(AddPerformersToChatsInviteCommandHandler)} chatBid.ChatId =  {chatBid.ChatId}");
        if (!chatBid.Accounts.Any(x => x.AccountId == item.AccountId))
        {
          chatBid.Accounts.Add(new Domains.Entities.AccountChat
          {
            AccountId = item.AccountId,
            ChatId = chatBid.ChatId,
            UnionUserDate = DateTime.UtcNow
          });
        }

        if (chatPurchase != null)
        {
          Log.Information($"{nameof(AddPerformersToChatsInviteCommandHandler)} chatPurchase.ChatId =  {chatPurchase.ChatId}");

          if (!chatPurchase.Accounts.Any(x => x.AccountId == item.AccountId))
          {
            chatPurchase.Accounts.Add(new Domains.Entities.AccountChat
            {
              AccountId = item.AccountId,
              ChatId = chatPurchase.ChatId,
              UnionUserDate = DateTime.UtcNow
            });
          }
        }
      }

      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }
  }
}
