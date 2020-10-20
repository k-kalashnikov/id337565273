namespace P.Messenger.Application.Accounts.Models
{
    public class AccountForMessengerDTO
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        //public static AccountForMessengerDTO Create(Account model)
        //    => new AccountForMessengerDTO
        //    {
        //        AccountId = model.AccountID,
        //        FirstName = model.FirstName,
        //        LastName = model.LastName,
        //        Login = model.Email
        //    };

        //public static AccountForMessengerDTO[] Create(IEnumerable<Account> models)
        //    => models.Select(x => Create(x)).ToArray();
    }
}