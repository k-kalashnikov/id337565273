using System;
using MediatR;
using SP.Messenger.Application.Accounts.Builders;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SP.Messenger.Application.Accounts.Commands
{
    public class SaveAccountCommandHandler : IRequestHandler<SaveAccountCommand, long>
    {
        private readonly MessengerDbContext _context;
        public SaveAccountCommandHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<long> Handle(SaveAccountCommand request, CancellationToken cancellationToken)
        {
            var accountDb = await _context.Accounts.FirstOrDefaultAsync(
                x=>x.AccountId==request.AccountId, cancellationToken);
            if (accountDb != null)
                return accountDb.AccountId;
            
            var account = Create.Account(request.AccountId,request.Login,request.FirstName,
                request.LastName,request.MiddleName).Build();

            var model = await _context.Accounts.AddAsync(account, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return model.Entity.AccountId;
        }
    }
}
