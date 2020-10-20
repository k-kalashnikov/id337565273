using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Chats.Models
{
    public class ChatTypeDTO
    {
        public int ChatTypeId { get; set; }
        public string Mnemonic { get; set; }
        public string Description { get; set; }

        public static ChatTypeDTO Create(ChatType model)
        {
            return new ChatTypeDTO
            {
                ChatTypeId = model?.ChatTypeId ?? 0,
                Mnemonic = model?.Mnemonic,
                Description = model?.Description
            };
        }
    }
}
