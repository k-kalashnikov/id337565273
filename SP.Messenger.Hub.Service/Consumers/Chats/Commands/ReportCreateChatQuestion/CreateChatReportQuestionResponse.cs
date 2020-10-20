namespace SP.Consumers.Models
{
    public class CreateChatReportQuestionResponse
    {
        public long ChatId { get; set; }
        public static CreateChatReportQuestionResponse Create(long chatId)
            => new CreateChatReportQuestionResponse {ChatId = chatId};
    }
}