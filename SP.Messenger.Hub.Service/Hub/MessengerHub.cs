using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.NewIdProviders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using SP.Consumers.Models;
using SP.Market.Core.Extension;
using SP.Market.Core.Paging;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Chats.Queries.GetChatById;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Chats.Queries.GetChatsByDocumentId;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Filters;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Models;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Queries;
using SP.Messenger.Application.Messages.Command.AttachedFile;
using SP.Messenger.Application.Messages.Command.Pined;
using SP.Messenger.Application.Messages.Command.Remove;
using SP.Messenger.Application.Messages.Queries.GetMessageById;
using SP.Messenger.Application.Messages.Queries.GetMessagesChat;
using SP.Messenger.Application.MessengerAssistent.Commands;
using SP.Messenger.Application.Voting.Commands.SetVote;
using SP.Messenger.Application.Voting.Queries.GetStateVoting;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Hub.Service.Services;
using JsonExtension = SP.Messenger.Common.Extensions.JsonExtension;
using ModuleName = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Hub
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class MessengerHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly IRequestClient<GetAccountIdentityRequest> _accountService;
        private readonly IBusControl _bus;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IActiveUsersService _activeUsersService;

        public MessengerHub(IBusControl bus,
            IRequestClient<GetAccountIdentityRequest> accountService, IMediator mediator,
            ICurrentUserService currentUserService, IHttpContextAccessor contextAccessor,
            IActiveUsersService activeUsersService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _currentUserService = currentUserService;
            _contextAccessor = contextAccessor;
            _activeUsersService = activeUsersService ?? throw new ArgumentNullException(nameof(activeUsersService));
        }

        #region system methods

        public override async Task OnConnectedAsync()
        {
            Log.Information($"*****{nameof(OnConnectedAsync)}: user: {Context.User.Identity.Name} ConnectionId: {Context.ConnectionId}");
            await _activeUsersService.AddUser(Context.ConnectionId, Context.User.Identity.Name);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _activeUsersService.RemoveUser(Context.User.Identity.Name, Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
            Log.Information($"*****{nameof(OnDisconnectedAsync)}: user: {Context.User.Identity.Name} ConnectionId: {Context.ConnectionId}");
        }

        public async Task<bool> IsSubscribed(string email)
        {
            var user = await _activeUsersService.GetUsersByEmail(email);
            return (user?.Login ?? string.Empty) == email;
        }

        #endregion

        #region public methods

        public async Task SaveMessage(MessageClient message)
        {
            var applicationUser = await GetCurrentAccountAsync();
            var messageEvent = await BuildMessageAsync(message, applicationUser);
            Log.Information($"SaveMessage");
            Log.Information($"SaveMessage Publish- : {messageEvent.ToJson()}");
            await _bus.Publish(messageEvent, CancellationToken.None);
            Log.Information($"SaveMessage - Finish");
        }

        public async Task GetMessagesChat(long chatId)
        {
            var messages = await GetMessagesByChatAsync(chatId);

            if (messages != null)
            {
                await Clients
                    .Client(this.Context.ConnectionId)
                    .SendAsync("onGetMessages", messages, CancellationToken.None);
            }
        }

        public async Task<GetChatsByObjectIdResponse[]> GetChats(string moduleName, Guid objectId)
        {
            Log.Information($"get -chats request: {objectId}");
            var response = GetChatsTree((await GetChatAsync(objectId)));
            Log.Information($"get -chats response: {response.ToJson()}");
            return response;
        }

        public async Task<ChatCollections> GetChatsBy(PageContextRequest<ChatFilter> request)
        {
            Log.Information($"{nameof(GetChatsBy)}: {request?.ToJson()}");
            var applicationUser = await GetCurrentAccountAsync();
            var response = await GetChatByParamAsync(applicationUser.AccountId, request);
            Log.Information($"{nameof(GetChatsBy)}: {response.ToJson()}");
            return response;
        }

        public async Task<ProcessingResult<bool>> RemoveMessage(long messageId)
        {
            var applicationUser = await GetCurrentAccountAsync();
            var command = RemoveMessageCommand.Create(applicationUser.AccountId, messageId);
            var result = await _mediator.Send(command);

            return new ProcessingResult<bool>(result.Result,
                result.Errors.Select(x => new SimpleResponseError(x.Message)));
        }

        public async Task<ProcessingResult<bool>> PinMessage(long messageId)
        {
            var command = PinMessageCommand.Create(messageId);
            var result = await _mediator.Send(command);

            return new ProcessingResult<bool>(result.Result,
                result.Errors.Select(x => new SimpleResponseError(x.Message)));
        }

        public async Task<ProcessingResult<bool>> Like(Guid votingId, Guid variantId, bool? like, string comment = null)
        {
            var command = SetVoteCommand.Create(votingId, variantId, like, comment);
            var result = await _mediator.Send(command);

            if (!result.Result)
                return new ProcessingResult<bool>(false);

            var query = GetStateVotingQuery.Create(votingId);
            var stateVotingObjects = await _mediator.Send(query);
            if (stateVotingObjects is null)
                return new ProcessingResult<bool>(false);

            var chat = await _mediator.Send(GetChatByIdQuery.Create(stateVotingObjects.ChatId));
            if (chat is null)
                return new ProcessingResult<bool>(false, new[] { new SimpleResponseError("Чат не найден") });

            var users = await _activeUsersService.GetUsersByEmail(chat.Accounts);
            var connectedIds = users.SelectMany(x => x.ConnectionIds).ToArray();
            await Clients
                .Clients(connectedIds)
                .SendAsync
                    ("onUpdateVotingMessages", stateVotingObjects, CancellationToken.None);
            return result;
        }

        public async Task<ProcessingResult<long>> AttachedFile(long chatId, string link)
        {
            var result = await _mediator.Send(AttachedFileCommand.Create(chatId, link));
            var message = await _mediator.Send(GetMessageByIdQuery.Create(result.Result));
            if (message is null)
                return new ProcessingResult<long>(0);

            var chat = await _mediator.Send(GetChatByIdQuery.Create(message.ChatId));
            if (chat is null)
                return new ProcessingResult<long>(0, new[] { new SimpleResponseError("Чат не найден") });

            var messages = new[] { message };

            var users = await _activeUsersService.GetUsersByEmail(chat.Accounts);
            var connectedIds = users.SelectMany(x => x.ConnectionIds).ToArray();

            await Clients
                .Clients(connectedIds)
                .SendAsync("onGetMessages", messages, CancellationToken.None);

            return result;
        }
        #endregion

        #region private methods

        private GetChatsByObjectIdResponse[] GetChatsTree(GetChatsByObjectIdResponse[] chatsByObjectIdResponses, long? _parentId = null)
        {
            var result = new Dictionary<long, GetChatsByObjectIdResponse>();

            if (chatsByObjectIdResponses.Length == 0)
            {
                Log.Warning("GetChatsTree: chats count 0");
                return result.Values.ToArray();
            }

            foreach (var item in chatsByObjectIdResponses)
            {
                if (result.ContainsKey(item.ChatId))
                {
                    continue;
                }
                if (item.ParentId == _parentId)
                {
                    item.Childs = GetChatsTree(chatsByObjectIdResponses, item.ChatId).OrderBy(m => m.ChatId).ToList();
                    result.Add(item.ChatId, item);
                }
            }

            if (((result.Count == 0) && (_parentId == null)))
            {
                return GetChatsTree(chatsByObjectIdResponses, chatsByObjectIdResponses.OrderBy(m => m.ChatId).First().ParentId);
            }

            return result.Values.ToArray();
        }

        private async Task<ApplicationUser> GetCurrentAccountAsync()
        {
            var accountRequest = GetAccountIdentityRequest.Create(Context.User.Identity.Name);

            Log.Information($"account-request: {Context.User.Identity.Name}");

            var jwt = _contextAccessor.HttpContext?.Request?.Headers
                ?.FirstOrDefault(x => x.Key == "Authorization").Value;
            Log.Information($"request-headers-jwt: {jwt}");

            if (string.IsNullOrEmpty(jwt))
                jwt = _contextAccessor?.HttpContext?.Request?.Query["access_token"];

            if (!string.IsNullOrEmpty(jwt))
                jwt = jwt.Value.ToString().Replace("Bearer ", string.Empty);

            Log.Information($"GetCurrentAccount get jwt: {jwt}");

            var currentUser = _currentUserService.CreateUserByToken(jwt);
            Log.Information($"currentUser SecurityToken: {currentUser.SecurityToken}");

            var response = await _accountService.GetResponse<GetAccountIdentityResponse>(accountRequest);
            Log.Information($"GetCurrentAccount: {response.ToJson()}");

            return ApplicationUser.Create(response?.Message);
        }

        private async Task<MessageClient[]> GetMessagesByChatAsync(long chatId)
        {
            var applicationUser = await GetCurrentAccountAsync();

            var command = GetMessagesChatQuery.Create(applicationUser.AccountId, chatId);
            var messages = await _mediator.Send(command);
            return messages;
        }

        private async Task<MessengerServerEvent> BuildMessageAsync(MessageClient message, ApplicationUser applicationUser)
        {
            Log.Information($"{nameof(BuildMessageAsync)} START");
            Log.Information($"{nameof(MessageClient)} {JsonExtension.ToJson(message)}");
            message.Date = DateTime.UtcNow;

            var account = Market.EventBus.RMQ.Shared.Events.Account.Create
            (
                id: applicationUser.AccountId,
                email: applicationUser.Login,
                firstName: applicationUser.FirstName,
                lastName: applicationUser.LastName,
                middleName: applicationUser.MiddleName,
                organizationId: applicationUser.OrganizationId,
                connetionId: Context.ConnectionId
            );
            Log.Information($"{nameof(Market.EventBus.RMQ.Shared.Events.Account)} : {JsonExtension.ToJson(account)}");

            var userCommand = message.Commands.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Command));
            MessageCommand command;
            if (userCommand is null)
                command = null;
            else
                command = MessageCommand.Create(userCommand.Command, userCommand.Value,
                    userCommand.DisplayName, userCommand.BotMessageType, AvailabilityCommand.Open);

            var messageType = (MessageType)Enum.Parse(typeof(MessageTypeClient), message.MessageType.ToString());

            var chat = await GetChatByIdAsync(message.ChatId);

            Log.Information($"chat: {chat?.ToJson()}");

            var informationChat = InformationChat.Create
            (
                documentId: chat?.DocumentId ?? Guid.Empty,
                chatId: message.ChatId,
                chatTypeMnemonic: string.Empty,
                parentDocumentId: chat?.ParentDocumentId,
                parentChatId: chat?.ParentId,
                parentChatTypeMnemonic: null,
                documentStatusMnemonic: chat?.DocumentStatus,
                module: chat?.Module,
                contractorId: chat?.ContractorId
            );
            Log.Information($"informationChat:  {informationChat?.ToJson()}");

            var moduleName =
                (Market.EventBus.RMQ.Shared.Events.ModuleName)Enum.Parse(typeof(ModuleName),
                    message.ModuleName.ToString());

            if (moduleName == 0)
                moduleName = (Market.EventBus.RMQ.Shared.Events.ModuleName)Enum.Parse(typeof(ModuleName),
                    chat?.Module ?? string.Empty);

            Log.Information($"moduleName:  {moduleName.ToString()}");
            var header = Header.Create(account, informationChat, moduleName,
                messageType, command, ButtonCommandCollection.Empty);
            Log.Information($"header:  {header.ToJson()}");

            var msg = MessengerServerEvent.Create(header, account.ConnectionId, string.Empty, message);
            Log.Information($"{nameof(MessengerServerEvent)} : {JsonExtension.ToJson(msg)}");
            Log.Information($"{nameof(BuildMessageAsync)} FINISH");
            return msg;
        }

        private async Task<GetChatsByObjectIdResponse[]> GetChatAsync(Guid documentId)
        {
            var query = GetChatsByDocumentIdQuery.Create(documentId);
            var result = await _mediator.Send(query);

            return GetChatsByObjectIdResponse.Create(result);
        }
        private async Task<ChatCollections> GetChatByParamAsync(long accountId, PageContextRequest<ChatFilter> request)
        {
            var query = GetChatsPageQuery.Create(request, accountId);
            var result = await _mediator.Send(query);

            return result;
        }

        private async Task<ChatShortDTO> GetChatByIdAsync(long chatId)
        {
            var query = GetChatByIdQuery.Create(chatId);
            var result = await _mediator.Send(query);

            return result;
        }

        #endregion
    }
}
