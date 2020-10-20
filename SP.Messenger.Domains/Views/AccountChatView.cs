namespace SP.Messenger.Domains.Views
{
    public class AccountChatView
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long ChatId { get; set; }
        public bool IsDisabled { get; set; }

        public static string View => "accountchatview";
    }
}
