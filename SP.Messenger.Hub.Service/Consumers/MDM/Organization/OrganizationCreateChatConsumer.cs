using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Infrastructure.Services.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MN = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers
{
  public class OrganizationCreateChatConsumer : IConsumer<OrganizationCreatedEvent>
  {
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
    private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;


    public OrganizationCreateChatConsumer(IMediator mediator,
      IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
      IRequestClient<GetUsersByRoleRequest> clientRoleRequest)
    {
      _mediator = mediator;
      _clientOrgRequest = clientOrgRequest;
      _clientRoleRequest = clientRoleRequest;
    }


    public async Task Consume(ConsumeContext<OrganizationCreatedEvent> context)
    {

      Log.Information($"{nameof(OrganizationCreateChatConsumer)} invoked");

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

      var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.OrganizationID), context.CancellationToken);
      accounts.AddRange(
        responseByOrg.Message.Accounts
        .Where(m => !accounts.Any(a => a.AccountId.Equals(m.Id)))
        .Select(m => new AccountMessengerDTO()
        {
          AccountId = m.Id,
          FirstName = m.FirstName,
          LastName = m.LastName,
          MiddleName = m.MiddleName,
          Login = m.Login
        })
      );


      var command = CreateChatCommand.Create(request.Name,
          "module.mdm.chat.organization.chat.public",
          true,
          request.OrganizationGUID,
          7,
          string.Empty,
          MN.Market,
          accounts);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = request.Name,
        ChatId = resultCommand.Result,
        ParentDocumentId = null,
        ChatTypeMnemonic = "module.mdm.chat.organization.chat.public"
      };
      await context.RespondAsync(CreateChatResponse.Create(response));
    }
  }
}
