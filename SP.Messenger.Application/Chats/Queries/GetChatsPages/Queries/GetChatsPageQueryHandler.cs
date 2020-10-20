using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Models;
using SP.Messenger.Common.Settings;
using SP.Messenger.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Queries.GetChatsPages.Queries
{
    public class GetChatsPageQueryHandler : IRequestHandler<GetChatsPageQuery, ChatCollections>
    {
        private readonly MessengerDbContext _context;
        private readonly IOptions<Settings> _options;
        private readonly ICurrentUserService _currentUserService;

        public GetChatsPageQueryHandler(MessengerDbContext context, IOptions<Settings> options, ICurrentUserService currentUserService)
        {
            _context = context;
            _options = options;
            _currentUserService = currentUserService;
        }

        public async Task<ChatCollections> Handle(GetChatsPageQuery request, CancellationToken cancellationToken)
        {
            var user = _currentUserService.GetCurrentUser();
            IQueryable<Domains.Views.ChatView> chats = null;
            int pageIndex = request.Request.PageIndex == 0 ? 1 : request.Request.PageIndex;

            chats = from view in _context.ChatView.AsNoTracking()
                    join accounts in _context.AccountChatView.AsNoTracking()
                    on view.ChatId equals accounts.ChatId
                    where accounts.Login.Equals(user.Login)
                    select view;

            if (!string.IsNullOrEmpty(request.Request.Filter.Module))
            {
                chats = from view in _context.ChatView.AsNoTracking()
                        where view.Module.Equals(request.Request.Filter.Module)
                        select view;
            }

            if (request.Request.Filter.DocumentId is null)
            {
                chats = chats.Where(x => x.ParentId == null);
            }
            else
            {
                chats = chats
                   .Where(x => x.DocumentId.Equals(request.Request.Filter.DocumentId.Value.ToString()))
                   .Union(chats
                   .Where(x => x.ParentDocumentId.Equals(request.Request.Filter.DocumentId.Value.ToString())));
            }

            var totalCount = chats.Count();
            chats = chats
                .Skip((pageIndex - 1) * request.Request.PageSize)
                .Take(request.Request.PageSize);

            return new ChatCollections
            {
                Data = ChatMessengerDTO.Create(await chats.ToArrayAsync(cancellationToken)),
                TotalCount = totalCount
            };
        }
    }
}
