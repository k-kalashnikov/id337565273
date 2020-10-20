namespace SP.Consumers.Models
{
    public class CreateChatReportProjectResponse
    {
        public long ChatId { get; set; }
        public static CreateChatReportProjectResponse Create(long chatId)
            => new CreateChatReportProjectResponse {ChatId = chatId};
    }
}