namespace SP.Consumers.Models
{
    public class GoogleNotificationMessageEvent
    {
        public GoogleNotificationMessageEvent(string title, string message, long[] accounts)
        {
            Title = title;
            Message = message;
            Accounts = accounts;
        }

        public string Title { get; }
        public string Message { get; }
        public long[] Accounts { get; }

        public static GoogleNotificationMessageEvent Create(string title, string message, long[] accounts)
            => new GoogleNotificationMessageEvent(title, message, accounts);
    }
}
