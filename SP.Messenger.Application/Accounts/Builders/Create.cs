namespace SP.Messenger.Application.Accounts.Builders
{
    public static class Create
    {
        public static AccountBuilder Account(long accountId, string login, string firstName, string lastName, string middleName)
            => new AccountBuilder(accountId, login, firstName, lastName, middleName);
    }
}
