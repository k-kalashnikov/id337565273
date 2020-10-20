namespace SP.Consumers.Models
{
    public class CloseBotMessageRequest
    {
        public long MessageId { get; set; }
        public dynamic Value { get; set; }
        public string Content { get; set; }
    }
}