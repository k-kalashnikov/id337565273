using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.FileStorage.Client.Services;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Queries.GetChatsByDocumentId
{
  public class GetChatsByDocumentIdQueryHandler : IRequestHandler<GetChatsByDocumentIdQuery, ChatMessengerDTO[]>
  {
    private readonly MessengerDbContext _context;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IFileStorageClientService _fileStorage;

    public GetChatsByDocumentIdQueryHandler(MessengerDbContext context,
      IHttpContextAccessor contextAccessor,
      IFileStorageClientService fileStorage)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
      _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
    }

    public async Task<ChatMessengerDTO[]> Handle(GetChatsByDocumentIdQuery request, CancellationToken cancellationToken)
    {
      Log.Information($"{nameof(GetChatsByDocumentIdQueryHandler)}");

      var serverInfo = await _fileStorage.GetServerInfoAsync(cancellationToken);

      var currentUserName = _contextAccessor.HttpContext?.User?.Identity?.Name;

      if (String.IsNullOrEmpty(currentUserName))
      {
        Log.Error($"{nameof(GetChatsByDocumentIdQueryHandler)} CALL THE POLICE! currentUserName is NULL! AGAIN!");
        return new List<ChatMessengerDTO>().ToArray();
      }

      Log.Information($"user: {currentUserName}");


      var documentId = request.DocumentId.ToString().ToLower();
      Log.Information($"documentId: {documentId}");


      var chatsParentsViews = await _context.ChatsParentsView
        .Where(c => c.DocumentId.ToLower().Equals(documentId))
        .ToArrayAsync(cancellationToken);

      Log.Information($"chatsParentsViews: {chatsParentsViews?.ToJson()}");


      var chatIds = chatsParentsViews?.Select(m => m.ChatId).ToList();

      var accountChats = await _context.AccountChatView
        .Where(m => chatIds.Contains(m.ChatId))
        .Where(m => m.Login.ToLower().Equals(currentUserName.ToLower()))
        .ToListAsync(cancellationToken);

      var chats = await _context.Chats
        .Where(m => accountChats.Select(x => x.ChatId).Contains(m.ChatId))
        .OrderBy(m => m.CreateAt)
        .ToListAsync(cancellationToken);


      var chatLastViews = await _context.LastMessagesView
          .Where(x => accountChats.Select(m => m.ChatId).Contains(x.ChatId))
          .OrderBy(m => m.CreateDate)
          .Include(x => x.Account)
          .Include(x => x.ChatType)
          .ToListAsync(cancellationToken);

      var result = new List<ChatMessengerDTO>();

      var chatsV = chats.Where(c => !chatLastViews.Any(clv => clv.ChatId.Equals(c.ChatId)));
      result.AddRange(ChatMessengerDTO.Create(chatLastViews, serverInfo.Host));
      result.AddRange(ChatMessengerDTO.Create(chatsV));

      if (result.Any(x => x.Mnemonic == "module.market.chat.vote"))
      {
        var chatsVoting = result
          .Where(x => x.Mnemonic == "module.market.chat.vote")
          .ToArray();
        
        var chatsVotingMessages = await _context.Chats
          .Where(m => chatsVoting.Select(x => x.ChatId).Contains(m.ChatId))
          .OrderByDescending(m => m.CreateAt)
          .Include(x => x.Messages)
          .ToListAsync(cancellationToken);
        
        foreach (var chatVotingMessages in chatsVotingMessages)
        {
          var message = chatVotingMessages.Messages.FirstOrDefault();
          var content = message?.Content?.FromJson<Consumers.Models.ContentMessage>();
          var votingId = content?.VotingContent?.VotingId ?? Guid.Empty;
          var voting = await _context.Votings.FirstOrDefaultAsync(x => x.Id == votingId, cancellationToken);
          var responseVariants = await _context.ResponseVariants
            .Where(x => x.VotingId == votingId)
            .ToArrayAsync(cancellationToken);

          var responseVariant = responseVariants.FirstOrDefault();
          var organizationsContent = responseVariant?.OrganizationsContent.FromJson<OrganizationContent>();
          var chatVoting = chatsVoting.FirstOrDefault(x => x.ChatId == chatVotingMessages.ChatId);
          var protocolNumber = organizationsContent?.Organizations.FirstOrDefault()?.ProtocolNumber;
          
          if(chatVoting is null)
            continue;
          
          chatVoting.Meta = new Metadata
          {
            ProtocolNumber = protocolNumber,
            ProtocolStartDate = chatVoting.CreateAt,
            IsClosed = voting?.IsClosed ?? false
          };
          if (protocolNumber != null)
          {
            chatVoting.Name = $"Комиссия №{protocolNumber}"; 
          }
        }
      }

      return result.OrderBy(m => m.ChatId).ToArray();
    }
  }
}
