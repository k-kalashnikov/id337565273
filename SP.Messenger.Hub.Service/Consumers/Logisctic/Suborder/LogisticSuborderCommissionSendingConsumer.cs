using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Logistic.Events.Events.Exchange;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;

namespace SP.Messenger.Hub.Service.Consumers
{
  public class LogisticSuborderCommissionSendingConsumer : IConsumer<CommissionSendingEvent>
  {

    private readonly IMediator _mediator;
    private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
    private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;

    public LogisticSuborderCommissionSendingConsumer(IMediator mediator,
      IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
      IRequestClient<GetUsersByRoleRequest> clientRoleRequest)
    {
      _mediator = mediator;
      _clientOrgRequest = clientOrgRequest;
      _clientRoleRequest = clientRoleRequest;
    }

    public async Task Consume(ConsumeContext<CommissionSendingEvent> context)
    {
      Log.Information($"{nameof(LogisticSuborderTradingStartConsumer)} invoked");
      var request = context.Message;

      var accounts = await GetAccountsAsync(context);
      var newGuid = Guid.NewGuid();


      var command = CreateChatCommand.Create("Комиссия",
        "module.logistic.chat.suborder.voting",
        true,
        newGuid,
        2,
        "",
        SP.Consumers.Models.ModuleName.Logistic,
        accounts,
        request.TransportSuborderId,
        null);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = "Комиссия",
        ChatId = resultCommand.Result,
        DocumentId = newGuid,
        ParentDocumentId = request.TransportSuborderId,
        ChatTypeMnemonic = "module.logistic.chat.suborder.voting"
      };

      Log.Information($"{nameof(LogisticSuborderTradingStartConsumer)} complited ChatId={resultCommand.Result}; ChatName=Комиссия");
    }

    private async Task<List<AccountMessengerDTO>> GetAccountsAsync(ConsumeContext<CommissionSendingEvent> context)
    {
      var request = context.Message;
      var result = new List<AccountMessengerDTO>();

      var responseByRoleSuper = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("superuser.module.platform") { }, context.CancellationToken);
      result.AddRange(responseByRoleSuper.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));

      var tempAccountsByRole = new List<AccountMessengerDTO>();

      var responseByRoleLog = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("LOGIST") { }, context.CancellationToken);
      tempAccountsByRole.AddRange(responseByRoleLog.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));

      var responseByRoleAdminLog = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("admin.module.logistic") { }, context.CancellationToken);
      tempAccountsByRole.AddRange(responseByRoleAdminLog.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));

      var responseByFinControlFstLog = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("FIN_CONTRL_1") { }, context.CancellationToken);
      tempAccountsByRole.AddRange(responseByFinControlFstLog.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));

      var responseByFinControlSndLog = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("FIN_CONTRL_2") { }, context.CancellationToken);
      tempAccountsByRole.AddRange(responseByFinControlSndLog.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.AccountId,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Email
      }));




      var tempAccountsByOrg = new List<AccountMessengerDTO>();
      var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.OrganizationId), context.CancellationToken);
      tempAccountsByOrg.AddRange(responseByOrg.Message.Accounts.Select(m => new AccountMessengerDTO()
      {
        AccountId = m.Id,
        FirstName = m.FirstName,
        LastName = m.LastName,
        MiddleName = m.MiddleName,
        Login = m.Login
      })
      );

      result.AddRange(tempAccountsByOrg.Where(o => tempAccountsByRole.Any(r => r.AccountId.Equals(o.AccountId))));
      return result;
    }
  }
}

//namespace SP.Logistic.Events.Events.Exchange
//{
//  /// <summary>
//  /// Событие отправки на комиссию
//  /// </summary>
//  public class CommissionSendingEvent
//  {
//    /// <summary>
//    /// Идентификатор субзаявки
//    /// </summary>
//    public Guid TransportSuborderId { get; set; }

//    /// <summary>
//    /// Идентификатор логистической заявки
//    /// </summary>
//    public Guid ParentId { get; set; }

//    /// <summary>
//    /// Краткое название СЗ
//    /// </summary>
//    public string Name { get; set; }

//    /// <summary>
//    /// Идентификатор организации
//    /// </summary>
//    public long OrganizationId { get; set; }
//  }
//}
