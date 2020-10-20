using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Logistic.Events.Events.TransportOrder;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;

namespace SP.Messenger.Hub.Service.Consumers
{
  public class LogisticOrderCreateConsumer : IConsumer<TransportOrderCreatedEvent>
  {
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
    private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;

    public LogisticOrderCreateConsumer(IMediator mediator,
      IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
      IRequestClient<GetUsersByRoleRequest> clientRoleRequest)
    {
      _mediator = mediator;
      _clientOrgRequest = clientOrgRequest;
      _clientRoleRequest = clientRoleRequest;
    }

    public async Task Consume(ConsumeContext<TransportOrderCreatedEvent> context)
    {
      Log.Information($"{nameof(LogisticOrderCreateConsumer)} invoked");
      var request = context.Message;

      Log.Information($"{nameof(LogisticOrderCreateConsumer)}, input data: {JsonConvert.SerializeObject(request)}");

      var accounts = await GetAccountsAsync(context);


      var command = CreateChatCommand.Create(request.TransportOrderName,
        "module.logistic.chat.order",
        true,
        request.TransportOrderId,
        2,
        "",
        SP.Consumers.Models.ModuleName.Logistic,
        accounts,
        null,
        null);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = request.TransportOrderName,
        ChatId = resultCommand.Result,
        DocumentId = request.TransportOrderId,
        ParentDocumentId = null,
        ChatTypeMnemonic = "module.logistic.chat.order"
      };

      Log.Information($"{nameof(LogisticOrderCreateConsumer)} DocumentId={request.TransportOrderId}");
      Log.Information($"{nameof(LogisticOrderCreateConsumer)} complited ChatId={resultCommand.Result}; ChatName={request.TransportOrderName}");

    }

    private async Task<List<AccountMessengerDTO>> GetAccountsAsync(ConsumeContext<TransportOrderCreatedEvent> context)
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
//  /// Событие создания заявки в системе
//  /// </summary>
//  public class TransportOrderCreatedEvent
//  {

//    /// <summary>
//    /// Создает событие создания заявки в системе
//    /// </summary>
//    public TransportOrderCreatedEvent(Guid transportOrderId, string transportOrderName, long organizationId)
//    {
//      TransportOrderId = transportOrderId;
//      TransportOrderName = transportOrderName;
//      OrganizationId = organizationId;
//    }

//    /// <summary>
//    /// Идентификатор заявки
//    /// </summary>
//    public Guid TransportOrderId { get; }

//    /// <summary>
//    /// Идентификатор организации
//    /// </summary>
//    public long OrganizationId { get; }

//    /// <summary>
//    /// Краткое имя заявки
//    /// </summary>
//    public string TransportOrderName { get; }

//    /// <summary>
//    /// Создает событие создания заявки в системе
//    /// </summary>
//    public static TransportOrderCreatedEvent Create(Guid transportOrderId, string transportOrderName, long organizationId)
//        => new TransportOrderCreatedEvent(transportOrderId, transportOrderName, organizationId);
//  }
//}
