using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;
using System.Linq;

namespace SP.Consumers.Models
{
    public class AccountMessengerDTO
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public static AccountMessengerDTO Create(Account model)
            => new AccountMessengerDTO
            {
                AccountId = model?.AccountId ?? 0,
                Login = model?.Login,
                FirstName = model?.FirstName,
                LastName = model?.LastName,
                MiddleName = model?.MiddleName
            };

        public static AccountMessengerDTO[] Create(Account[] models)
            => models.Select(x => Create(x)).ToArray();

        public static AccountMessengerDTO Create(AccountChatView model)
           => new AccountMessengerDTO
           {
               AccountId = model.AccountId,
               Login = model.Login,
               FirstName = model.FirstName,
               LastName = model.LastName,
               MiddleName = model.MiddleName
           };

        public static AccountMessengerDTO[] Create(AccountChatView[] models)
            => models.Select(x => Create(x)).ToArray();

        //AccountForMessengerDTO 
        public static AccountMessengerDTO Create(SP.Consumers.Models.AccountForMessengerDTO model)
           => new AccountMessengerDTO
           {
               AccountId = model?.AccountId ?? 0,
               Login = model?.Login,
               FirstName = model?.FirstName,
               LastName = model?.LastName,
               MiddleName = model?.MiddleName
           };

        public static AccountMessengerDTO[] Create(SP.Consumers.Models.AccountForMessengerDTO[] models)
            => models?.Select(x => Create(x)).ToArray();

        //GetAccountsByIdsResponse 
        public static AccountMessengerDTO Create(SP.Consumers.Models.AccountDto model)
           => new AccountMessengerDTO
           {
               AccountId = model?.AccountId ?? 0,
               Login = model?.Email,
               FirstName = model?.FirstName,
               LastName = model?.LastName,
               MiddleName = model?.MiddleName
           };

        public static AccountMessengerDTO[] Create(SP.Consumers.Models.GetAccountsByIdsResponse models)
            => models.Accounts?.Select(x => Create(x)).ToArray();
    }
}
