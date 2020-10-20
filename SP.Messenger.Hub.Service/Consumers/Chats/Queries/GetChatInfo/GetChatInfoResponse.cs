
namespace SP.Consumers.Models
{
    public class GetChatInfoResponse
    {
        public GetChatInfoResponse(long chatId, string name, string mnemonic)
        {
            ChatId = chatId;
            Name = name;
            Mnemonic = mnemonic;
        }
        public long ChatId { get; set; }
        public string Mnemonic { get; set; }
        public string Name { get; set; }
        
        
        public static GetChatInfoResponse Create(ChatLightDTO model)
        => new GetChatInfoResponse(model.ChatId, model.Name, model.Mnemonic);
    }
}