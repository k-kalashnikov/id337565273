
using SP.Messenger.Application.Chats.Models;

namespace SP.Consumers.Models
{
    public class ChatTypeResponse
    {
        public int ChatTypeId { get; set; }
        public string Mnemonic { get; set; }
        public string Description { get; set; }

        public static ChatTypeResponse Create(ChatTypeDTO model)
            => new ChatTypeResponse
            {
                ChatTypeId = model?.ChatTypeId ?? 0,
                Mnemonic = model?.Mnemonic,
                Description = model?.Description
            };
    }
}
