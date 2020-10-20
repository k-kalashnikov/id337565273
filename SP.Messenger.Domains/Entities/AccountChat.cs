using System;

namespace SP.Messenger.Domains.Entities
{
    public class AccountChat
    {
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime UnionUserDate { get; set; }
    }
}
