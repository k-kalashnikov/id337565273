using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAllAccounts
{
    public class GetAllAccountsQueryHandler : IRequestHandler<GetAllAccountsQuery, AccountMessengerDTO[]>
    {
        private readonly MessengerDbContext _context;
        public GetAllAccountsQueryHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<AccountMessengerDTO[]> Handle(GetAllAccountsQuery request, CancellationToken cancellationToken)
        {
            var models = await _context.Accounts
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return AccountMessengerDTO.Create(models);
        }
    }
}
