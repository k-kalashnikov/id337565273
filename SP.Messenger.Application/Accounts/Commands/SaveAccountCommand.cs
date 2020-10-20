using MediatR;

namespace SP.Messenger.Application.Accounts.Commands
{
    public class SaveAccountCommand : IRequest<long>
    {
        public SaveAccountCommand(long accountId, string login, string firstName,string lastName,string middleName)
        {
            AccountId = accountId;
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }
        public long AccountId { get; }
        public string Login { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        
        public static SaveAccountCommand Create(long accountId, string login, string firstName,string lastName,string middleName)
            => new SaveAccountCommand(accountId,  login, firstName, lastName, middleName);
    }
}
