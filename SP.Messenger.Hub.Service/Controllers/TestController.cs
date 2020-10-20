using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Consumers.Models;
using SP.Market.Core.ApiVersion;
using SP.Market.Identity.Client.Attributes;
using SP.Market.Identity.Client.Factory;
using SP.Messenger.Application.Chats.Commands.AddPerformersToChatsInvite;
using SP.Messenger.Application.Chats.Commands.CreateVoteAutoChat;
using SP.Messenger.Application.Messages.Command.AttachedFile;
using SP.Messenger.Application.Messages.Queries.GetMessageById;
using SP.Messenger.Application.Voting.Commands.CreateVoting;
using SP.Messenger.Application.Voting.Commands.SetVote;
using SP.Messenger.Application.Voting.Queries;
using SP.Messenger.Application.Voting.Queries.GetStateVoting;
using SP.Market.Core.Paging;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Filters;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Queries;
using SP.Messenger.Application.Messages.Command.PurchaseUpdateStatusToOffersCollect;
using Microsoft.AspNetCore.Authorization;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Messages.Queries.GetMessagesChat;
using Serilog;
using SP.Market.Identity.Common.Interfaces;
using MassTransit;
using JsonExtension = SP.Messenger.Common.Extensions.JsonExtension;
using SP.Market.Core.Extension;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Persistence;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Events.Requests;
using SP.Messenger.Client.Responces;
using SP.Purchase.Events.Purchase;
using System.Threading;
using SP.Messenger.Application.Messages.Command;
using messenger = SP.Messenger.Application.Messages.Builders;
using SP.Contract.Client.Interfaces;
using MongoDB.Bson.IO;

namespace SP.Messenger.Hub.Service.Controllers
{
    [ApiController]
    [ApiVersion(VersionController.Version10)]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/test")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRequestClient<GetAccountIdentityRequest> _accountService;
        private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
        private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private readonly IRequestClient<CreateProtocolCommissionRequest> _clientProtocolCommisionRequest;
        private readonly MessengerDbContext _context;
        private readonly IContractClientService _contractClientService;

        public TestController(IMediator mediator,
          ICurrentUserService currentUserService,
          IHttpContextAccessor contextAccessor,
          IRequestClient<GetAccountIdentityRequest> accountService,
          IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
          IRequestClient<GetUsersByRoleRequest> clientRoleRequest,
          IRequestClient<CreateProtocolCommissionRequest> clientProtocolCommisionRequest,
          MessengerDbContext context,
          IContractClientService contractClientService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _contextAccessor = contextAccessor;
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _clientOrgRequest = clientOrgRequest;
            _clientRoleRequest = clientRoleRequest;
            _context = context;
            _clientProtocolCommisionRequest = clientProtocolCommisionRequest;
            _contractClientService = contractClientService;
        }

        [HttpPost, Route("voting")]
        public async Task<ActionResult> PostVoting([FromBody] CreateVotingAutoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("like")]
        public async Task<ActionResult> PostLike([FromBody] SetVoteCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpPost, Route("voting-create-chat")]
        public async Task<ActionResult> PostCreateChat([FromBody] CreateVoteAutoChatCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet, Route("voting-chat/{votingId}")]
        public async Task<ActionResult> GetChat(Guid votingId)
        {
            var query = GetStateVotingQuery.Create(votingId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost, Route("attached-file")]
        public async Task<ActionResult> PostCreateChat([FromBody] AttachedFileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet, Route("get-message/{messageId}")]
        public async Task<ActionResult> GetMessage(long messageId)
        {
            var query = GetMessageByIdQuery.Create(messageId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost, Route("add-performers-to-chats-invite-command-test")]
        public async Task<ActionResult> AddPerformersToChatsInviteCommandTest(AddPerformersToChatsInviteCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost, Route("get-chats-by")]
        public async Task<ActionResult> GetChatBy(PageContextRequest<ChatFilter> request)
        {
            var query = GetChatsPageQuery.Create(request, 12135);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost, Route("PurchaseUpdateStatusToOffersCollectTest")]
        public async Task<ActionResult> PurchaseUpdateStatusToOffersCollectTest(PurchaseUpdateStatusToOffersCollectCommand _command)
        {
            //var command = PurchaseUpdateStatusToOffersCollectCommand.Create(Guid.Parse("d7f7c646-dcf8-4e4b-9d3d-17fb5072d5fa"), "PurchaseUpdateStatusToOffersCollectTest comment", 22221);
            var result = await _mediator.Send(_command);
            return Ok(result);
        }


        [HttpPost, Route("GetVotingQueryTest")]
        public async Task<ActionResult> GetVotingQueryTest([FromBody] GetChatsQuery getChats)
        {
            var chats = await _mediator.Send(getChats);
;
            if (chats == null) 
            {
                return BadRequest("chats not found");
            }


            var chatIds = chats
                .DistinctBy(m => m.ChatId)
                .Where(m => m.Mnemonic?.Equals("module.market.chat.vote") ?? false)
                .Select(m => m.ChatId)
                .ToList();

            if (chatIds == null)
            {
                return BadRequest("voting chats not found");
            }

            var messages = await _context.Messages
                .Where(m => chatIds.Contains(m.ChatId))
                .ToListAsync();

            var contents = messages
                .Select(m => Newtonsoft.Json.JsonConvert.DeserializeObject<ContentMessage>(m.Content))
                .ToList();

            var votingIds = contents
                .Select(m => m.VotingContent?.VotingId)
                .Where(v => (v ?? Guid.Empty) != Guid.Empty)
                .ToList();

            var votings = await _context.Votings
                .Where(v => votingIds.Contains(v.Id))
                .ToListAsync();

            votings.ForEach(v =>
            {
                v.IsClosed = true;
                _context.Votings.Update(v);
            });

            _context.SaveChangesAsync();

            return Ok(votings);
        }

        [HttpPost, Route("GetChatsQueryTest")]
        public async Task<ActionResult> GetChatsQueryTest([FromBody] GetChatsQuery getChats)
        {
            var result = await _mediator.Send(getChats);
            return Ok(result);
        }

        [HttpPost, Route("GetChatMessagesQueryTest")]
        public async Task<ActionResult> GetChatMessagesQueryTest([FromBody] GetMessagesChatQuery getMessagesChat)
        {
            var result = await _mediator.Send(getMessagesChat);
            return Ok(result);
        }

        [HttpPost, Route("GetStateVotingQueryTest")]
        public async Task<ActionResult> GetStateVotingQueryTest([FromBody] GetStateVotingQuery getStateVoting)
        {
            var result = await _mediator.Send(getStateVoting);
            return Ok(result);
        }

        [HttpGet, Route("GetChatsTest")]
        public async Task<ActionResult<GetChatsByObjectIdResponse[]>> GetChats(Guid objectId)
        {
            Log.Information($"get -chats request: {objectId}");
            var applicationUser = await GetCurrentAccountAsync();
            var response = GetChatsTree((await GetChatAsync(applicationUser.AccountId, objectId)));
            Log.Information($"get -chats response: {response.ToJson()}");
            return response;
        }

        [HttpPost, Route("PurchaseCreateTest")]
        public async Task<ActionResult<ChatInfoResponse>> PurchaseCreateTest(CreatedPurchaseEvent createdPurchaseEvent)
        {
            Log.Information($"{nameof(PurchaseCreateTest)} invoked");
            var request = createdPurchaseEvent;




            var accounts = new List<AccountMessengerDTO>();

            var responseByRole = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("superuser.module.platform") { });
            accounts.AddRange(responseByRole.Message.Accounts.Select(m => new AccountMessengerDTO()
            {
                AccountId = m.AccountId,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                Login = m.Email
            }));

            var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.OrganizationId));
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

            var resultCommand = await _mediator.Send(command);
            var response = new ChatInfoResponse
            {
                ChatName = request.PurchaseName,
                ChatId = resultCommand.Result,
                DocumentId = request.PurchaseId,
                ParentDocumentId = null,
                ChatTypeMnemonic = "module.bidCenter.chat.common"
            };
            return response;
        }

        [HttpGet, Route("UpdateVotingMessages")]
        public async Task<ActionResult> UpdateVotingMessages()
        {
            var chats = await _context.Chats
              .Include(m => m.Accounts)
              .Where(m => m.ChatTypeId == 11)
              .ToListAsync();

            Log.Information($"{nameof(UpdateVotingMessages)} find {chats.Count} voting chats");

            foreach (var item in chats)
            {
                Log.Information($"ChatName {item.Name}; ChatId {item.ChatId};");
            }

            var chatsView = await _context.ChatView
              .Where(cv => chats.Select(c => c.ChatId).Contains(cv.ChatId))
              .ToListAsync();

            Log.Information($"{nameof(UpdateVotingMessages)} find {chatsView.Count} voting chatsView");

            foreach (var item in chatsView)
            {
                Log.Information($"{nameof(UpdateVotingMessages)} ChatName {item.Name}; ChatId {item.ChatId}; DocumentId {item.DocumentId}");
                var tempChat = chats.FirstOrDefault(c => c.ChatId.Equals(item.ChatId));
                if (tempChat == null)
                {
                    Log.Error($"{nameof(UpdateVotingMessages)} chat not found");
                    continue;
                }

                Log.Information($"{nameof(UpdateVotingMessages)} Chat {tempChat.ChatId} have {tempChat.Accounts.Count} accounts");
                try
                {
                    var result = await _clientProtocolCommisionRequest.GetResponse<CreateProtocolCommissionResponse>(new CreateProtocolCommissionRequest()
                    {
                        ProtocolId = Guid.Parse(item.DocumentId),
                        Accounts = tempChat.Accounts.Select(m => m.AccountId)
                    });

                    Log.Information($"{nameof(UpdateVotingMessages)} result = {result.Message.Result}");
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }

            }



            return new ObjectResult(chats);
        }

        [HttpPost, Route("PurchaseRefused")]
        public async Task<ActionResult> PurchaseRefused([FromBody] PurchaseRefusedEvent purchaseRefusedEvent) 
        {
            var request = purchaseRefusedEvent;

            var currentUser = _contextAccessor.HttpContext.User;
            if (currentUser == null)
            {
                return BadRequest("currentUser is null");
            }

            var author = await _context.Accounts.FirstOrDefaultAsync(m => m.Login.ToLower().Equals(currentUser.Identity.Name.ToLower()), CancellationToken.None) ?? new Domains.Entities.Account()
            {
                AccountId = 1,
                FirstName = "�����",
                Login = "stec.superuser@mail.ru",
                MiddleName = "",
                LastName = "������������"
            };


            var getChatByDocumentQuery = GetChatsQuery.Create(author.AccountId, request.PurchaseId);
            var resultGetChatByDocumentQuery = await _mediator.Send(getChatByDocumentQuery, CancellationToken.None);

            foreach (var item in resultGetChatByDocumentQuery.Where(m => !m.ParentChatId.HasValue).DistinctBy(m => m.ChatId))
            {
                var messageClient = new MessageClient
                {
                    Commands = Array.Empty<CommandClient>(),
                    Content = $"{author?.LastName ?? ""} {author?.FirstName ?? ""} {author?.MiddleName ?? ""}  ��������� �� ���������� ������� � ������� {request.PurchaseStatus} �� ������� {request.RefuseReason}",
                    Date = DateTime.UtcNow,
                    DocumentId = item.DocumentId,
                    File = null,
                    ButtonCommands = Array.Empty<ButtonCommand>(),
                    ModuleName = ModuleName.Market,
                    ChatTypeMnemonic = item.Mnemonic,
                    ChatId = item.ChatId,
                    MessageId = 1,
                    Author = new Author
                        (
                            author.AccountId,
                            author.Login,
                            author.FirstName,
                            author.LastName,
                            author.MiddleName
                        ),
                    MessageType = MessageTypeClient.User,
                    Edited = false,
                    Pined = false,
                    Readed = false,
                    VotingClient = null
                };

                var message = messenger.Create.Message
                (
                    chatId: item.ChatId,
                    accountId: author.AccountId,
                    messageTypeId: 5,
                    content: messageClient
                );
                var saveMessageCommand = SaveMessageCommand.Create(message, author.Login);
                var resultSaveMessageCommand = await _mediator.Send(saveMessageCommand, CancellationToken.None);
            }

            return Ok();
        }

        [HttpGet, Route("state-voting/{votingId}")]
        public async Task<ActionResult> GetStateVoting(Guid votingId)
        {
            var query = GetStateVotingQuery.Create(votingId);
            return Ok(await _mediator.Send(query));
        }

        private async Task<ApplicationUser> GetCurrentAccountAsync()
        {
            var accountRequest = GetAccountIdentityRequest.Create(User.Identity.Name);

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

        private async Task<GetChatsByObjectIdResponse[]> GetChatAsync(long accountId, Guid documentId)
        {
            var query = GetChatsQuery.Create(accountId, documentId);
            var result = await _mediator.Send(query);

            return GetChatsByObjectIdResponse.Create(result);
        }

        private GetChatsByObjectIdResponse[] GetChatsTree(GetChatsByObjectIdResponse[] _getChatsByObjectIdResponses, long? _parentId = null)
        {
            var result = new Dictionary<long, GetChatsByObjectIdResponse>();
            foreach (var item in _getChatsByObjectIdResponses)
            {
                if (result.ContainsKey(item.ChatId))
                {
                    continue;
                }
                if (item.ParentId == _parentId)
                {
                    item.Childs = GetChatsTree(_getChatsByObjectIdResponses, item.ChatId);
                    result.Add(item.ChatId, item);
                }
            }
            return result.Values.ToArray();
        }
    }
}
