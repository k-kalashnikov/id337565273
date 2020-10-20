using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;

namespace SP.Messenger.Hub.Service.Consumers
{
  public class PurchaseCreateConsumer : IConsumer<CreatedPurchaseEvent>
  {

    private readonly IMediator _mediator;
    private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
    private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;

    public PurchaseCreateConsumer(IMediator mediator,
      IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
      IRequestClient<GetUsersByRoleRequest> clientRoleRequest)
    {
      _mediator = mediator;
      _clientOrgRequest = clientOrgRequest;
      _clientRoleRequest = clientRoleRequest;
    }

    public async Task Consume(ConsumeContext<CreatedPurchaseEvent> context)
    {
      Log.Information($"{nameof(PurchaseCreateConsumer)} invoked");
      var request = context.Message;


      var accounts = new List<AccountMessengerDTO>();

      var responseByRole = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("superuser.module.platform") { }, context.CancellationToken);
      accounts.AddRange(responseByRole.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));

      var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.OrganizationId), context.CancellationToken);
      accounts.AddRange(
        responseByOrg.Message.Accounts
        .Where(m => !accounts.Any(a => a.AccountId.Equals(m.Id)))
        .Select(m => new AccountMessengerDTO() {
          AccountId = m.Id,
          FirstName = m.FirstName,
          LastName = m.LastName,
          MiddleName = m.MiddleName,
          Login = m.Login
        })
      );



      var command = CreateChatCommand.Create(request.PurchaseName,
              "module.bidCenter.chat.common",
              true,
              request.PurchaseId,
              3,
              "",
              SP.Consumers.Models.ModuleName.Market,
              accounts,
              null,
              null);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = request.PurchaseName,
        ChatId = resultCommand.Result,
        DocumentId = request.PurchaseId,
        ParentDocumentId = null,
        ChatTypeMnemonic = "module.bidCenter.chat.common"
      };
      await context.RespondAsync(CreateChatResponse.Create(response));
    }
  }
}
