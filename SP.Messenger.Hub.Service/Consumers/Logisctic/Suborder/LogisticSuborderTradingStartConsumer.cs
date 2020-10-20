using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Logistic.Events.Events.Exchange;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class LogisticSuborderTradingStartConsumer : IConsumer<TradingStartEvent>
    {

        private readonly IMediator _mediator;
        private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
        private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private readonly IRequestClient<GetOrganizationsRequest> _clientOrgsRequest;

        public LogisticSuborderTradingStartConsumer(IMediator mediator,
          IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
          IRequestClient<GetUsersByRoleRequest> clientRoleRequest,
          IRequestClient<GetOrganizationsRequest> clientOrgsRequest)
        {
            _mediator = mediator;
            _clientOrgRequest = clientOrgRequest;
            _clientRoleRequest = clientRoleRequest;
            _clientOrgsRequest = clientOrgsRequest;
        }

        public async Task Consume(ConsumeContext<TradingStartEvent> context)
        {
            Log.Information($"{nameof(LogisticSuborderTradingStartConsumer)} invoked");
            var request = context.Message;

            var getOrgsResponse = await _clientOrgsRequest.GetResponse<GetOrganizationsResponse[]>(GetOrganizationsRequest.Create(request.PerformerIds.ToArray()), context.CancellationToken);
            var perfomers = getOrgsResponse.Message.ToDictionary(m => m.OrganizationId, m => m);

            Log.Information($"{nameof(LogisticSuborderTradingStartConsumer)} message {JsonConvert.SerializeObject(request)}");


            foreach (var item in request.PerformerIds)
            {
                var accounts = await GetAccountsAsync(context, item);

                if (perfomers[item] == null)
                {
                    Log.Error($"{nameof(LogisticSuborderTradingStartConsumer)} Organization with id={item} not exist");
                }

                var chatName = (perfomers[item] != null) ? perfomers[item].Name : "Неизвестная организация";
                var newGuid = Guid.NewGuid();


                var command = CreateChatCommand.Create(chatName,
                  "module.logistic.chat.suborder.performer",
                  true,
                  newGuid,
                  2,
                  "",
                  SP.Consumers.Models.ModuleName.Logistic,
                  accounts,
                  request.SuborderId,
                  null);

                var resultCommand = await _mediator.Send(command, context.CancellationToken);
                var response = new ChatInfoResponse
                {
                    ChatName = chatName,
                    ChatId = resultCommand.Result,
                    DocumentId = newGuid,
                    ParentDocumentId = request.SuborderId,
                    ChatTypeMnemonic = "module.logistic.chat.suborder.performer"
                };

                Log.Information($"{nameof(LogisticSuborderTradingStartConsumer)} complited ChatId={resultCommand.Result}; ChatName={chatName}");

            }
        }

        private async Task<List<AccountMessengerDTO>> GetAccountsAsync(ConsumeContext<TradingStartEvent> context, long performerId)
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

            var responseByOrgPer = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(performerId), context.CancellationToken);
            result.AddRange(responseByOrgPer.Message.Accounts.Select(m => new AccountMessengerDTO()
            {
                AccountId = m.Id,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                Login = m.Login
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
            }));

            result.AddRange(tempAccountsByOrg.Where(o => tempAccountsByRole.Any(r => r.AccountId.Equals(o.AccountId))));
            return result;
        }
    }
}


////Добавил чтобы структуру не переносить - обновят пакет SP.Logistic.Events - уберу
//namespace SP.Logistic.Events.Events.Exchange
//{
//  /// <summary>
//  /// Событие начала торгов
//  /// </summary>
//  public class TradingStartEvent
//  {
//    /// <summary>
//    /// Идентификатор субзаявки, для которой начаты торги
//    /// </summary>
//    public Guid SuborderId { get; set; }

//    /// <summary>
//    /// Набор уникальных идентификаторов организаций-исполнителей
//    /// </summary>
//    public HashSet<long> PerformerIds { get; set; }

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
