using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds;
using SP.Messenger.Application.Accounts.Queries.GetAccountsFromPlatform;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsChat
{
    public class GetAccountByChatHandler : IRequestHandler<GetAccountByChat, AccountMessengerDTO[]>
    {
        private readonly MessengerDbContext _context;
     
        private readonly IMediator _mediator;

        public GetAccountByChatHandler(MessengerDbContext context, IMediator mediator)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<AccountMessengerDTO[]> Handle(GetAccountByChat request, CancellationToken cancellationToken)
        {
            AccountMessengerDTO[] accounts = null;
            var accountsView = await _context.AccountChatView
                          .AsNoTracking()
                          .Where(x => x.ChatId == request.ChatId)
                          .ToArrayAsync(cancellationToken);           

            if (accountsView is null || !accountsView.Any())
            {
                try
                {
                    var chat = await _context.Chats
                        .Include(x=>x.ChatType)
                        .FirstOrDefaultAsync(x => x.ChatId.Equals(request.ChatId), cancellationToken);

                    var query = GetAccountsByOrganizationIdsQuery.Create(
                        new[] { request.OrganizationId.Value },
                        chat?.ChatType?.Mnemonic ?? "module.bidCenter.chat.common");
                    accounts = await _mediator.Send(query, cancellationToken);
                    //accounts = await _mediator.Send(GetAccountsFromPlatformQuery.Create(request.OrganizationId), cancellationToken);
                }
                catch (Exception e)
                {
                    Log.Error($"Не смог загрузить пользователей из платформы: {e.ToString()}");
                }
            }
            else
                accounts = AccountMessengerDTO.Create(accountsView);

            return accounts;
        }
    }
}
