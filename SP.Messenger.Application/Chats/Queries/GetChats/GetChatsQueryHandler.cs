using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SP.FileStorage.Client.Services;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Common.Settings;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetChats
{
    public class GetChatsQueryHandler : IRequestHandler<GetChatsQuery, ChatMessengerDTO[]>
    {
        private readonly MessengerDbContext _context;
        private readonly IOptions<Settings> _options;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFileStorageClientService _fileStorage;

        public GetChatsQueryHandler(MessengerDbContext context,
            IOptions<Settings> options,
            IHttpContextAccessor contextAccessor,
            IFileStorageClientService fileStorage)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }
        public async Task<ChatMessengerDTO[]> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var serverInfo = await _fileStorage.GetServerInfoAsync(cancellationToken);
            var user = _contextAccessor.HttpContext?.User?.Identity?.Name;
            Log.Information($"{nameof(GetChatsQueryHandler)} user: {user}");

            var documentId = request.DocumentId.ToString().ToLower();

            Log.Information($"{nameof(GetChatsQueryHandler)} documentId: {documentId}");

            var chatsParentsViews = await _context.ChatsParentsView
                .Where(c => c.DocumentId.ToLower().Equals(documentId))
                .ToArrayAsync(cancellationToken);

            Log.Information($"{nameof(GetChatsQueryHandler)} {JsonConvert.SerializeObject(chatsParentsViews)}");

            var chats = await _context.LastMessagesView
                .Where(x => chatsParentsViews.Select(i => i.ChatId).Contains(x.ChatId))
                .Include(x => x.Account)
                .Include(x => x.ChatType)
                .ToListAsync(cancellationToken);

            foreach (var chat in chats)
                chat.Account.Chats = null;

            Log.Information($"{nameof(GetChatsQueryHandler)} chatsParentsViews: {chatsParentsViews?.ToJson()}");

            var chatsIds = chatsParentsViews.Select(i => i.ChatId).ToArray();
            Log.Information($"{nameof(GetChatsQueryHandler)} chatsIds: {chatsIds?.ToJson()}");

            var chatsDb = await _context.Chats
                .Where(x => chatsIds.Any(i => x.ChatId == i))
                .ToArrayAsync(cancellationToken);

            if (chats is null)
            {
                chats = new List<LastMessagesView>();
            }

            chats.AddRange(chatsDb.Select(item => new LastMessagesView
            {
                Account = null,
                Content = null,
                Mnemonic = item.ChatType?.Mnemonic,
                Name = item.Name,
                ChatId = item.ChatId,
                CreateDate = DateTime.MinValue,
                DocumentId = request.DocumentId,
                ParentId = item.ParentId,
                ChatType = item.ChatType,
                ChatTypeId = item.ChatTypeId,
                AccountId = 0,
                MessageId = 0,
                MessageTypeId = 0
            }));


            if (user != null)
            {
                var accountsChat2 = await _context.AccountChatView
                    .Where(x => chats.Select(i => i.ChatId).Contains(x.ChatId))
                    .ToArrayAsync(cancellationToken);

                Log.Information($"accountsChat2: {accountsChat2?.ToJson()}");

                var chatsV = new List<LastMessagesView>();
                foreach (var chat in chats)
                {
                    var accounts = accountsChat2.Where(x => x.ChatId == chat.ChatId).ToArray();
                    if (accounts.Any())
                    {
                        if (accounts.FirstOrDefault(x => x.Login.ToLower().Equals(user.ToLower())) != null)
                        {
                            try
                            {
                                Log.Information($"chat: {chat?.ToJson()}");
                                chatsV.Add(chat);
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex.ToString());
                            }
                        }
                    }
                }
                Log.Information($"chatsV: {chatsV?.ToJson()}");
                return ChatMessengerDTO.Create(chatsV, serverInfo.Link).DistinctBy(m => m.ChatId).ToArray();
            }
            Log.Information($"chats: {chats?.ToJson()}");
            return ChatMessengerDTO.Create(chats, serverInfo.Link).DistinctBy(m => m.ChatId).ToArray();
        }
    }
}
