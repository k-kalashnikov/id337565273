using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Commands.SaveAccountRange
{
    public class SaveAccountRangeCommandHandler : IRequestHandler<SaveAccountRangeCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;
        public SaveAccountRangeCommandHandler(MessengerDbContext contex)
        {
            _context = contex;
        }

        public async Task<ProcessingResult<bool>> Handle(SaveAccountRangeCommand request, CancellationToken cancellationToken)
        {
            var accountsDb = await _context.Accounts.ToListAsync(cancellationToken);
            
            var accountsNewList = new List<Account>();

            accountsNewList = CheckAccounts(request, accountsDb, accountsNewList);

            var result = 0;
            try
            {
                await _context.Accounts.AddRangeAsync(accountsNewList, cancellationToken);
                result = await _context.SaveChangesAsync(cancellationToken);
            }
            catch (System.Exception ex)
            {
                ;
            }
            return new ProcessingResult<bool>(result > 0);
        }

        private List<Account> CheckAccounts(SaveAccountRangeCommand request,
            List<Account> accountsDb,
            List<Account> accounts)
        {
            foreach (var item in request.Accounts)
            {
                if (accountsDb.FirstOrDefault(x => x.AccountId == item.AccountId) is null)
                    accounts.Add(item);
            }

            return accounts;
        }
    }
}
