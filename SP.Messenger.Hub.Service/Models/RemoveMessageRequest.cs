namespace SP.Consumers.Models
{
    public class RemoveMessageRequest
    {
        public RemoveMessageRequest(long accountId, long messageId)
        {
            AccountId = accountId;
            MessageId = messageId;
        }
        public long AccountId { get;}
        public long MessageId { get; }
        public static RemoveMessageRequest Create(long accountId, long messageId)
            => new RemoveMessageRequest(accountId, messageId);
    }
}