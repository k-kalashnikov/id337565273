namespace SP.Consumers.Models
{
    public class GetChatTypeRequest
    {
        public GetChatTypeRequest(long chatId)
        {
            ChatId = chatId;
        }
        public long ChatId { get; }
        public static GetChatTypeRequest Create(long chatId)
            => new GetChatTypeRequest(chatId);
    }
}
