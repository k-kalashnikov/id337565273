using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Accounts.Builders
{
    public class AccountBuilder
    {
        private readonly Account _account;
        public AccountBuilder(long accountId, string login, string firstName, string lastName,string middleName)
        {
            _account = new Account();
            _account.AccountId = accountId;
            _account.Login = login;
            _account.FirstName = firstName;
            _account.LastName = lastName;
            _account.MiddleName = middleName;
        }

        public Account Build()
        {
            return _account;
        }
    }
}
