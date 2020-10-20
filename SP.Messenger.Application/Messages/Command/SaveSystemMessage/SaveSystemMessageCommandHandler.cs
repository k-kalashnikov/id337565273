using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using SP.Consumers.Models;
using SP.Market.Core.Extension;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Messages.Builders;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Common.Settings;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence;
using Account = SP.Messenger.Domains.Entities.Account;
using ButtonCommand = SP.Consumers.Models.ButtonCommand;
using CommandClient = SP.Consumers.Models.CommandClient;
using ModuleName = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Application.Messages.Command.SaveSystemMessage
{
    public class SaveSystemMessageCommandHandler : IRequestHandler<SaveSystemMessageCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;
        private readonly IOptions<Settings> _options;
        private readonly IBusControl _bus;
        public SaveSystemMessageCommandHandler(MessengerDbContext context,
            IOptions<Settings> options, IBusControl bus)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task<ProcessingResult<bool>> Handle(SaveSystemMessageCommand request, CancellationToken cancellationToken)
        {           
            var chat = await GetChat(request.DocumentId, request.Mnemonic, cancellationToken);
            if (chat is null)
            {
                Log.Error($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetChat)} chat is null");
                return new ProcessingResult<bool>(true, new []{ new SimpleResponseError($"chat is null") });
            }
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetChat)} chat: {chat.ToJson()}");
            
            var document = await GetDocument(request.DocumentId,cancellationToken);
            if (document is null)
            {
                Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetDocument)} document is null");
                return new ProcessingResult<bool>(true, new []{ new SimpleResponseError($"document is null") });
            }
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetDocument)} document: {document.ToJson()}");
            
            var module = GetModule(document);
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetModule)} module: {module.ToJson()}");
            var account = await GetAccount(request.AccountId);
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(GetAccount)} account: {account?.ToJson()}");
            var message = await BuildMessage(request, module, chat, request.Mnemonic, account);
            if (message is null)
            {
                Log.Error($"{nameof(SaveSystemMessageCommandHandler)} {nameof(BuildMessage)} message is null");
                return new ProcessingResult<bool>(true, new []{ new SimpleResponseError($"message is null") });
            }
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(BuildMessage)} message: {message.ToJson()}");
            
            var entity = await _context.Messages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var messageContent = message.Content.FromJson<Domains.Entities.ContentMessage>();
            var @event = await BuildEvent(account, document.DocumentId, chat.ChatId,
                request.Mnemonic, module, messageContent.Content, entity.Entity.MessageId);
            await _bus.Publish(@event, cancellationToken);
            
            return new ProcessingResult<bool>(true);
        }

        private async Task<Message> BuildMessage(SaveSystemMessageCommand request, ModuleName module, ChatView chat,
            string mnemonic, Account account)
        {
            var content = TemplateMessage(request.Message, account);
            var messageUser = await BuildMessageClient(content, request.DocumentId,
                module, chat.ChatId, mnemonic, 0);

            return Create.Message(chat.ChatId, 999999, 2, messageUser);
        }

        private async Task<MessageClient> BuildMessageClient(string content, Guid documentId, 
            ModuleName moduleName,long chatId, string mnemonic, long messageId)
        {
            var module = (ModuleName)Enum.Parse(typeof(ModuleName), moduleName.ToString());
            var account = await GetAccount(999999);
            return new MessageClient
            {
                Commands = Array.Empty<CommandClient>(),
                Content = content,
                Date = DateTime.UtcNow,
                DocumentId = documentId,
                File = null,
                ButtonCommands = Array.Empty<ButtonCommand>(),
                ModuleName = module,
                ChatTypeMnemonic = mnemonic,
                ChatId = chatId,
                MessageId = messageId,
                MessageType = MessageTypeClient.User,
                Author = new Author(account?.AccountId??0, account?.Login, account?.FirstName, account?.LastName, account?.MiddleName)
            };
        }

        private async Task<ChatView> GetChat(Guid documentId, string mnemonic, CancellationToken token)
        {
            return await _context.ChatView.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.DocumentId.Equals(documentId.ToString())
                    && x.Mnemonic.Equals(mnemonic), token);
        }
        private async Task<Document> GetDocument(Guid documentId, CancellationToken token)
        {
            return await _context.Documents.AsNoTracking()
                .Where(x => x.DocumentId.Equals(documentId))
                .Include(x=>x.DocumentType)
                .FirstOrDefaultAsync(token);
        }
        private static ModuleName GetModule(Document document)
        {
            return Enum.Parse<ModuleName>(document.DocumentType.Name);
        }

        private async Task<Account> GetAccount(long accountId)
        {
            return  await _context.Accounts.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.AccountId.Equals(accountId)) ?? await _context.Accounts
                        .FirstOrDefaultAsync(x => x.AccountId.Equals(999999));
        }

        private string TemplateMessage(string message, Account account)
        {
            var template = _options.Value.Templates.SystemMessage;
            return template.Replace("%info%", message).Replace("%author%",
                $"{account.LastName} {account.FirstName} {account.LastName}".TrimEnd());
        }

        private async Task<MessengerClientEvent> BuildEvent(Account account, Guid documentId, long chatId, string mnemonic, 
            ModuleName module, string content,  long messageId)
        {
            var accountHeader = SP.Market.EventBus.RMQ.Shared.Events.Account.Create(account.AccountId,
                account.Login,
                account.FirstName,
                account.LastName,
                account.MiddleName,
                0,
                string.Empty);
            
            var informationChat = InformationChat.Create(documentId, chatId, 
                mnemonic, null, null, null);
            
            var moduleName = (SP.Market.EventBus.RMQ.Shared.Events.ModuleName)Enum.Parse(typeof(SP.Market.EventBus.RMQ.Shared.Events.ModuleName),
                module.ToString());
            
            var header = Header.Create(accountHeader, informationChat, moduleName,
                SP.Market.EventBus.RMQ.Shared.Events.MessageType.System, 
                null, ButtonCommandCollection.Empty);

            var message = await BuildMessageClient(content, documentId,
                module, chatId, mnemonic, messageId);
                
            var clientEvent = new MessengerClientEvent(header, "", string.Empty, message); 
            Log.Information($"{nameof(SaveSystemMessageCommandHandler)} {nameof(BuildEvent)} = {clientEvent.ToJson()}");
            return clientEvent;
        }
    }
}