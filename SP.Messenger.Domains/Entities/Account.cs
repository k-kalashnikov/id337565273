using System.Collections.Generic;

namespace SP.Messenger.Domains.Entities
{
    public class Account
    {
        public Account()
        {
            Chats = new HashSet<AccountChat>();
        }
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool IsDisabled { get; set; }
        public ICollection<AccountChat> Chats { get; set; }
    }
}
