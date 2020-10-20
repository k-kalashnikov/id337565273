using MediatR;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Accounts.Commands.SaveAccountRange
{
    public class SaveAccountRangeCommand : IRequest<ProcessingResult<bool>>
    {
        public SaveAccountRangeCommand(Account[] accounts)
        {
            Accounts = accounts;
        }

        public Account[] Accounts { get; }
        
        public static SaveAccountRangeCommand Create(Account[] accounts)
            => new SaveAccountRangeCommand(accounts);
    }
}
