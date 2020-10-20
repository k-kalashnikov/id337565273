using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Commands.SaveAccountRange;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Infrastructure.Services.Report;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccounts
{
    public class UpdateAccountsCommandHandler : IRequestHandler<UpdateAccountsCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly MessengerDbContext _context;
        
        private readonly IContragentsProject<GetContragentsByProjectRequest,
            GetContragentsByProjectResponse> _projectService;

        private readonly IAccountsOrganizationService<GetAccountsByOrganizationRequest,
            GetAccountsByOrganizationResponse[]> _accountsService;
        
        public UpdateAccountsCommandHandler(IContragentsProject<GetContragentsByProjectRequest,GetContragentsByProjectResponse> projectService,
            IAccountsOrganizationService<GetAccountsByOrganizationRequest, GetAccountsByOrganizationResponse[]> accountsService,
            IMediator mediator,
            MessengerDbContext context)
        {
            _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
            _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(UpdateAccountsCommand request, CancellationToken cancellationToken)
        {
            var query = GetContragentsByProjectRequest.Create(request.ProjectId);
            var partners = await _projectService.GetContrgaentsByProject(query, cancellationToken);

            var queryAccounts = GetAccountsByOrganizationRequest.Create(partners.ContragentsIds);
            var accounts = await _accountsService.GetAccountByOrganizationIds(queryAccounts, cancellationToken);
            
            Log.Information($"GetAccountByOrganizationIds: collection count: {accounts.Length}");
            if (!accounts.Any())
                return false;
            
            await SaveAccounts(accounts, cancellationToken);
            Log.Information($"GetAccountByOrganizationIds: {nameof(SaveAccounts)} {accounts.ToJson()}");
            
            var chatDb = await _context.Chats
                .FirstOrDefaultAsync(x=>x.ChatId.Equals(request.ChatId), cancellationToken);

            var accountsChatDb = await _context
                .AccountChatView.Where(x => x.ChatId.Equals(request.ChatId))
                .ToArrayAsync(cancellationToken);

            var isContextChanged = false;
            var dateNow = DateTime.UtcNow;
            foreach (var account in accounts)
            {
                if (accountsChatDb?.FirstOrDefault(x => x.Login == account.Login) is null)
                {
                    isContextChanged = true;
                    chatDb.Accounts.Add(new AccountChat
                    {
                        ChatId = request.ChatId, 
                        AccountId = account.AccountId,
                        UnionUserDate = dateNow
                    });
                }
            }

            if (isContextChanged)
            {
                Log.Information($"SaveChangesAsync: collection {nameof(AccountChat)}: {chatDb.Accounts.ToJson()}");
                await _context.SaveChangesAsync(cancellationToken);   
            }
            
            return true;
        }
        
        private async Task<ProcessingResult<bool>> SaveAccounts(GetAccountsByOrganizationResponse[] accounts,
            CancellationToken cancellationToken)
        {
            var models = accounts?.Select(x => new Account
            {
                AccountId = x.AccountId,
                FirstName = x.FirstName,
                IsDisabled = false,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Login = x.Login
            }).ToArray();

            var command = SaveAccountRangeCommand.Create(models);
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
