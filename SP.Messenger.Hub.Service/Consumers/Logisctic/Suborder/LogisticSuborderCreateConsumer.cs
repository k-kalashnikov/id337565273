using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Logistic.Events.Events.TransportOrder;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;

namespace SP.Messenger.Hub.Service.Consumers
{
  public class LogisticSuborderCreateConsumer : IConsumer<TransportSuborderCreatedEvent>
  {
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
    private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;

    public LogisticSuborderCreateConsumer(IMediator mediator,
      IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
      IRequestClient<GetUsersByRoleRequest> clientRoleRequest)
    {
      _mediator = mediator;
      _clientOrgRequest = clientOrgRequest;
      _clientRoleRequest = clientRoleRequest;
    }

    public async Task Consume(ConsumeContext<TransportSuborderCreatedEvent> context)
    {
      Log.Information($"{nameof(LogisticSuborderCreateConsumer)} invoked");
      var request = context.Message;

      

      var accounts = await GetAccountsAsync(context);

      var command = CreateChatCommand.Create(request.Name,
        "module.logistic.chat.suborder",
        true,
        request.Id,
        2,
        "",
        SP.Consumers.Models.ModuleName.Logistic,
        accounts,
        request.ParentId,
        null);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = request.Name,
        ChatId = resultCommand.Result,
        DocumentId = request.Id,
        ParentDocumentId = request.ParentId,
        ChatTypeMnemonic = "module.logistic.chat.suborder"
      };

      Log.Information($"{nameof(LogisticSuborderCreateConsumer)} DocumentId={request.Id} ParentId={request.ParentId}");
      Log.Information($"{nameof(LogisticSuborderCreateConsumer)} complited ChatId={resultCommand.Result}; ChatName={request.Name}");
    }

    private async Task<List<AccountMessengerDTO>> GetAccountsAsync(ConsumeContext<TransportSuborderCreatedEvent> context)
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

////Добавил чтобы структуру не переносить - обновят пакет SP.Logistic.Events - уберу
//namespace SP.Logistic.Events.Events.TransportOrder
//{
//  /// <summary>
//  /// Событие создания логистической субзаявки
//  /// </summary>
//  public class TransportSuborderCreatedEvent
//  {
//    /// <summary>
//    /// Идентификатор логистической субзаявки
//    /// </summary>
//    public Guid Id { get; set; }

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
