using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Contract.Events.Request.GetOrganizationContracts;
using SP.Messenger.Application.Accounts.Commands;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Commands.AddNewAccountsToExistsChats
{
  public class AddNewAccountsToExistsChatsCommandHandler : IRequestHandler<AddNewAccountsToExistsChatsCommand, bool>
  {
    private readonly IMediator _mediator;
    private readonly MessengerDbContext _context;
    private readonly IAccountsOrganizationService<GetAccountsByOrganizationIdsRequest, GetAccountsByOrganizationIdsResponse> _accountsService;
    private readonly IRequestClient<GetRolesByAccountIdRequest> _clientGetRolesRequest;
    private readonly IRequestClient<GetOrganizationContractsRequest> _clientGetContractRequest;


    public AddNewAccountsToExistsChatsCommandHandler(IMediator mediator,
      MessengerDbContext context,
      IAccountsOrganizationService<GetAccountsByOrganizationIdsRequest, GetAccountsByOrganizationIdsResponse> accountsService,
      IRequestClient<GetRolesByAccountIdRequest> clientGetRolesRequest)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _accountsService = accountsService ?? throw new ArgumentNullException(nameof(accountsService));
      _clientGetRolesRequest = clientGetRolesRequest;
    }

    public async Task<bool> Handle(AddNewAccountsToExistsChatsCommand request, CancellationToken cancellationToken)
    {
      var accountId = await SaveNewAccount(request, cancellationToken);
      var result = await UpdateChats(request, accountId, cancellationToken);

      return result;
    }

    private async Task<long> SaveNewAccount(AddNewAccountsToExistsChatsCommand request, CancellationToken cancellationToken)
    {
      var saveUserCommand = SaveAccountCommand.Create(
          request.AccountId,
          request.Login,
          request.FirstName,
          request.LastName,
          request.MiddleName);

      var accountId = await _mediator.Send(saveUserCommand, cancellationToken);
      return accountId;
    }

    private async Task<bool> UpdateChats(AddNewAccountsToExistsChatsCommand request, long accountId, CancellationToken cancellationToken)
    {
      if (request.AccountId == default)
        throw new InvalidOperationException($"нет {nameof(request.AccountId)}: {request.AccountId}");

      var getRolesByAccountId = await _clientGetRolesRequest.GetResponse<GetRolesByAccountIdResponse>(new GetRolesByAccountIdRequest(accountId), cancellationToken);
      if (getRolesByAccountId.Message.Roles.Any(m => m.Mnemonic.ToLower().Equals("superuser.module.platform")))
      {
        return await AddAccountToChatAsAdmin(accountId, cancellationToken); ;
      }

      if (!request.Organization.HasValue)
      {
        Log.Error($"{nameof(AddNewAccountsToExistsChatsCommandHandler)} User isn't superuser and haven't OrganizationId");
        return false;
      }

      if (getRolesByAccountId.Message.Roles.Any(m => m.Mnemonic.ToLower().Equals("manager.module.market"))
          || getRolesByAccountId.Message.Roles.Any(m => m.Mnemonic.ToLower().Equals("contractor.module.platform")))
      {

      }

      var queryAccounts =
        new GetAccountsByOrganizationIdsRequest(new List<long> {request.Organization.Value}, new string[] { });
       
      var response = await _accountsService.GetAccountByOrganizationIds(queryAccounts, cancellationToken);
      await AddAccountToOrgsChat(accountId, response, cancellationToken, request);



      return true;
    }

    private async Task AddAccountToOrgsChat(long accountId,
        GetAccountsByOrganizationIdsResponse response,
        CancellationToken cancellationToken,
        AddNewAccountsToExistsChatsCommand request)
    {
      var accountsByOrgIds = response.Accounts.Select(x => x.Id).Distinct();

      accountsByOrgIds = accountsByOrgIds.Where(x => x != accountId).ToArray();
      
      var chats = await _context.AccountChatView
          .Where(x => !x.IsDisabled && accountsByOrgIds.Contains(x.AccountId))
          .Select(i => i.ChatId).Distinct()
          .ToArrayAsync(cancellationToken);

      var chatsDb = await _context.Chats
          .Where(x => chats.Contains(x.ChatId) && x.Name != "Комиссия")
          .Include(x=>x.Accounts)
          .ToArrayAsync(cancellationToken);

      foreach (var chat in chatsDb)
      {
        if(chat is null)
          continue;

        if (chat.Name == "Комиссия")
          continue;
        
        if (chat.Accounts.FirstOrDefault(x => x.AccountId == accountId) != null)
          continue;

        chat.Accounts.Add(new AccountChat
        {
          ChatId = chat.ChatId,
          AccountId = accountId,
          UnionUserDate = DateTime.Now
        });
      }

      await _context.SaveChangesAsync(cancellationToken);

      //Проверка на существование чата организации
      var chatOrgDb = await _context.ChatView
          .Where(x => x.DocumentId.Equals(request.OrganizationGuid))
          .ToArrayAsync(cancellationToken);

      if (chatOrgDb != null && chatOrgDb.Any())
      {
        foreach (var item in chatOrgDb)
        {
          var chatOrg = await _context.Chats.FirstOrDefaultAsync(m => m.ChatId.Equals(item.ChatId));
          chatOrg.Accounts.Add(new AccountChat()
          {
            ChatId = chatOrg.ChatId,
            AccountId = accountId,
            UnionUserDate = DateTime.Now
          });
        }
        await _context.SaveChangesAsync(cancellationToken);
      }
    }

    private async Task AddAccountToContractChat(long accountId,
      long organizationId,
      CancellationToken cancellationToken)
    {
      var getContracts = await _clientGetContractRequest.GetResponse<GetOrganizationContractsResponse>(new GetOrganizationContractsRequest(organizationId), cancellationToken);
      if ((getContracts.Message.Contracts == null) || (getContracts.Message.Contracts?.Count() == 0))
      {
        Log.Error($"{nameof(AddNewAccountsToExistsChatsCommandHandler)} this organization={organizationId} have not contracts");
        return;
      }

      var chats = await _context.ChatView
        .Where(m => getContracts.Message.Contracts.Select(c => c.Id).Any(c => c.Equals(m.DocumentId)))
        .ToListAsync(cancellationToken);

      var chatsDb = await _context.Chats
        .Where(m => chats.Select(c => c.ChatId).Contains(m.ChatId))
        .ToListAsync(cancellationToken);

      foreach (var item in chatsDb)
      {
        item.Accounts.Add(new AccountChat()
        {
          AccountId = accountId,
          ChatId = item.ChatId,
          UnionUserDate = DateTime.Now
        });;
      }

      await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> AddAccountToChatAsAdmin(long accountId, CancellationToken cancellationToken)
    {
      var chats = await _context.Chats
          .Where(x => !x.IsDisabled)
          .ToArrayAsync(cancellationToken);

      foreach (var chat in chats)
      {
        chat.Accounts.Add(new AccountChat
        {
          ChatId = chat.ChatId,
          AccountId = accountId,
          UnionUserDate = DateTime.Now
        });
      }

      await _context.SaveChangesAsync(cancellationToken);
      return true;
    }
  }
}
