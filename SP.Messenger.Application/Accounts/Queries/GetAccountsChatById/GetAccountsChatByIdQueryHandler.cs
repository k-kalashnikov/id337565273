using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsChatById
{
    public class GetAccountsChatByIdQueryHandler : IRequestHandler<GetAccountsChatByIdQuery, long[]>
    {
        private readonly MessengerDbContext _context;

        public GetAccountsChatByIdQueryHandler(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<long[]> Handle(GetAccountsChatByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Chats
                .Where(x => x.ChatId.Equals(request.ChatId))
                .SelectMany
                    (
                        x=>x.Accounts.Select(a=> a.Account.AccountId)
                    )
                .ToArrayAsync(cancellationToken);
        }
    }
}