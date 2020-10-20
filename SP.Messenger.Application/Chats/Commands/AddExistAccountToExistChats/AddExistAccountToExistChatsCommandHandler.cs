using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Commands.AddExistAccountToExistChats
{
    public class AddExistAccountToExistChatsCommandHandler : IRequestHandler<AddExistAccountToExistChatsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly MessengerDbContext _context;
        private readonly IAccountsOrganizationService<GetAccountsByOrganizationRequest, GetAccountsByOrganizationResponse[]> _accountsService;

        public AddExistAccountToExistChatsCommandHandler(IMediator mediator, MessengerDbContext context,
            IAccountsOrganizationService<GetAccountsByOrganizationRequest, GetAccountsByOrganizationResponse[]> accountsService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
        }

        public async Task<bool> Handle(AddExistAccountToExistChatsCommand request, CancellationToken cancellationToken)
        {
            var result = await UpdateChats(request, cancellationToken);

            return result;
        }

        private async Task<bool> UpdateChats(AddExistAccountToExistChatsCommand request, CancellationToken cancellationToken)
        {
            if (request.Roles.Contains("superuser.module.platform")) 
            {
                return await AddAccountToChatAsAdmin(request.AccountId, cancellationToken);
            }

            return true;
        }

        private async Task<bool> AddAccountToChatAsAdmin(long accountId, CancellationToken cancellationToken)
        {
            var chats = await _context.Chats
                .Where(x => !x.IsDisabled)
                .ToArrayAsync(cancellationToken);

            foreach (var chat in chats)
            {
                try
                {
                    chat.Accounts.Add(new AccountChat
                    {
                        ChatId = chat.ChatId,
                        AccountId = accountId,
                        UnionUserDate = DateTime.Now
                    });
                }
                catch (Exception e) 
                {
                    Log.Error(e.Message);
                    Log.Information($"This account {accountId} already exist in this chat {chat.ChatId}");
                }

            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
