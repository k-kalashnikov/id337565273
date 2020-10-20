using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;
using SP.Purchase.Events.Purchase;
using Account = SP.Messenger.Domains.Entities.Account;
using ButtonCommand = SP.Consumers.Models.ButtonCommand;
using CommandClient = SP.Consumers.Models.CommandClient;
using ContentMessage = SP.Consumers.Models.ContentMessage;
using messenger = SP.Messenger.Application.Messages.Builders;
using ModuleName = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class PurchaseRefusedConsumer : IConsumer<PurchaseRefusedEvent>
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly MessengerDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly IBusControl _bus;
        
        public PurchaseRefusedConsumer(IHttpContextAccessor contextAccessor,
            MessengerDbContext dbContext, IMediator mediator, IBusControl bus)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            _mediator = mediator;
            _bus = bus;
        }

        public async Task Consume(ConsumeContext<PurchaseRefusedEvent> context)
        {
            Log.Information($"{nameof(PurchaseRefusedConsumer)} invoked for message = {context.MessageId} with {context.Message.PurchaseId}");
            Log.Information($"{nameof(PurchaseRefusedConsumer)} PurchaseId={context.Message.PurchaseId}; SequenceNumber={context.Message.SequenceNumber}");

            var request = context.Message;

            var currentUser = _contextAccessor.HttpContext.User;
            if (currentUser is null)
            {
                Log.Error($"currentUser is null");
                return;
            }

            var author = await _dbContext
                .Accounts
                .FirstOrDefaultAsync(m => m.Login.ToLower().Equals(currentUser.Identity.Name.ToLower()), context.CancellationToken) ?? new Account()
            {
                AccountId = 1,
                FirstName = "Супер",
                Login = "stec.superuser@mail.ru",
                MiddleName = "",
                LastName = "Пользователь"
            };

            Log.Information($"{nameof(PurchaseRefusedConsumer)} authorID = {author.AccountId}");

            await SendMessagesAsync(author, request, context.CancellationToken);
            await RefuseVotingAsync(author, request, context.CancellationToken);
        }

        private async Task SendMessagesAsync(Account account, PurchaseRefusedEvent request, CancellationToken cancellationToken) 
        {
            var getChatByDocumentQuery = GetChatsQuery.Create(account.AccountId, request.PurchaseId);
            var resultGetChatByDocumentQuery = await _mediator.Send(getChatByDocumentQuery, cancellationToken);

            Log.Information($"{nameof(PurchaseRefusedConsumer)} chats = {string.Join(',', resultGetChatByDocumentQuery.Select(m => $"{m.ChatId}"))}");

            var chatMessengersCollection = resultGetChatByDocumentQuery
                .Where(m => !m.ParentChatId.HasValue)
                .DistinctBy(m => m.ChatId);
            
            foreach (var item in chatMessengersCollection)
            {
                var messageClient = new MessageClient
                {
                    Commands = Array.Empty<CommandClient>(),
                    Content = $"{account.LastName ?? ""} {account.FirstName ?? ""} {account.MiddleName ?? ""}  отказался от проведения закупки в статусе {request.PurchaseStatus} по причине {request.RefuseReason}",
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
                            account.AccountId,
                            account.Login,
                            account.FirstName,
                            account.LastName,
                            account.MiddleName
                        ),
                    MessageType = MessageTypeClient.User,
                    Edited = false,
                    Pined = false,
                    Readed = false,
                    VotingClient = null
                };

                var message = messenger.Create.Message
                (
                    item.ChatId,
                    account.AccountId,
                    messageTypeId: 5,
                    messageClient
                );
                var saveMessageCommand = SaveMessageCommand.Create(message, account.Login);
                await _mediator.Send(saveMessageCommand, cancellationToken);

                await SendMessageAsync(account, cancellationToken, item, messageClient);
            }
        }

        private async Task SendMessageAsync(Account account, CancellationToken cancellationToken, 
            ChatMessengerDTO item, MessageClient messageClient)
        {
            var document = await GetDocument(item.DocumentId, cancellationToken);
            var module = GetModule(document);

            var accountHeader = Market.EventBus.RMQ.Shared.Events.Account.Create(
                id: account.AccountId,
                email: account.Login,
                firstName: account.FirstName,
                lastName: account.LastName,
                middleName: account.MiddleName,
                organizationId: 0,
                connetionId: string.Empty);

            var informationChat = InformationChat.Create(
                documentId: item.DocumentId,
                chatId: item.ChatId,
                chatTypeMnemonic: item.Mnemonic,
                parentDocumentId: null,
                parentChatId: null,
                parentChatTypeMnemonic: null);

            var moduleName = (SP.Market.EventBus.RMQ.Shared.Events.ModuleName) Enum.Parse(
                typeof(SP.Market.EventBus.RMQ.Shared.Events.ModuleName),
                module.ToString());

            var header = Header.Create(accountHeader, informationChat, moduleName,
                Market.EventBus.RMQ.Shared.Events.MessageType.System,
                null, ButtonCommandCollection.Empty);

            var clientEvent = new MessengerClientEvent(header, string.Empty, string.Empty, messageClient);
            await _bus.Publish(clientEvent, cancellationToken);
        }

        private async Task<Document> GetDocument(Guid documentId, CancellationToken token)
            => await _dbContext.Documents.AsNoTracking()
                .Where(x => x.DocumentId.Equals(documentId))
                .Include(x=>x.DocumentType)
                .FirstOrDefaultAsync(token);
        private static ModuleName GetModule(Document document)
            => Enum.Parse<ModuleName>(document.DocumentType.Name);
        
        private async Task RefuseVotingAsync(Account account, PurchaseRefusedEvent request, CancellationToken cancellationToken) 
        {
            var getChatByDocumentQuery = GetChatsQuery.Create(account.AccountId, request.PurchaseId);
            var chats = await _mediator.Send(getChatByDocumentQuery, cancellationToken);

            if (chats == null)
            {
                Log.Error($"{nameof(PurchaseRefusedConsumer)} chats not found");
                return;
            }

            var chatIds = chats
                .DistinctBy(m => m.ChatId)
                .Where(m => m.Mnemonic?.Equals("module.market.chat.vote") ?? false)
                .Select(m => m.ChatId)
                .ToList();

            if (!chats.Any())
            {
                Log.Error($"{nameof(PurchaseRefusedConsumer)} chats with voting not found");
                return;
            }

            var messages = await _dbContext.Messages
                .Where(m => chatIds.Contains(m.ChatId))
                .ToListAsync(cancellationToken);

            if (messages is null || !messages.Any())
            {
                Log.Error($"{nameof(PurchaseRefusedConsumer)} messages with voting not found");
                return;
            }

            var contents = messages
                .Select(m => Newtonsoft.Json.JsonConvert.DeserializeObject<ContentMessage>(m.Content))
                .ToList();

            var votingIds = contents
                .Select(m => m.VotingContent?.VotingId)
                .Where(v => (v ?? Guid.Empty) != Guid.Empty)
                .ToList();

            var votings = await _dbContext.Votings
                .Where(v => votingIds.Contains(v.Id))
                .ToListAsync(cancellationToken);

            if (votings is null || !votings.Any())
            {
                Log.Error($"{nameof(PurchaseRefusedConsumer)} votings not found");
                return;
            }

            votings.ForEach(v =>
            {
                v.IsClosed = true;
            });
            
            _dbContext.Votings.UpdateRange(votings);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}