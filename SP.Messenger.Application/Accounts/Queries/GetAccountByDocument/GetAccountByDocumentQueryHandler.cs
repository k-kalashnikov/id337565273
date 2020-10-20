using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountByDocument
{
    public class GetAccountByDocumentQueryHandler : IRequestHandler<GetAccountByDocumentQuery, AccountMessengerDTO[]>
    {
        private readonly MessengerDbContext _context;
        public GetAccountByDocumentQueryHandler(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<AccountMessengerDTO[]> Handle(GetAccountByDocumentQuery request, CancellationToken cancellationToken)
        {
            var accounts = from accountsView in _context.AccountChatView.AsNoTracking()
                           join chats in _context.ChatView.AsNoTracking()
                           on accountsView.ChatId equals chats.ChatId
                           where chats.DocumentId == request.DocumentId.ToString()
                           select accountsView;

            return AccountMessengerDTO.Create(await accounts.ToArrayAsync(cancellationToken));

        }
    }
}
