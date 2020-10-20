using MediatR;
using SP.Market.Core.Paging;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Filters;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatsPages.Queries
{
    public class GetChatsPageQuery : IRequest<ChatCollections>
    {
        public GetChatsPageQuery(PageContextRequest<ChatFilter> request, long accountId)
        {
            Request = request;
            AccountId = accountId;
        }

        public PageContextRequest<ChatFilter> Request { get; }
        public long AccountId { get; }

        public static GetChatsPageQuery Create(PageContextRequest<ChatFilter> request, long accountId)
            => new GetChatsPageQuery(request, accountId);
    }
}
