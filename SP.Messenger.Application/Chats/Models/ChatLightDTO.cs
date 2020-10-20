using System.Collections.Generic;
using System.Linq;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;

namespace SP.Consumers.Models
{
    public class ChatLightDTO
    {
        public long ChatId { get; set; }
        public string Name { get; set; }
        public string Mnemonic { get; set; }
        public bool IsDisabled { get; set; }

        public static ChatLightDTO Create(Chat model)
            => new ChatLightDTO
            {
                ChatId = model?.ChatId ?? 0,
                Name = model?.Name,
                Mnemonic = ChatTypeDTO.Create(model?.ChatType)?.Mnemonic
            };
        
        public static ChatLightDTO Create(ChatView model)
            => new ChatLightDTO
            {
                ChatId = model?.ChatId ?? 0,
                Name = model?.Name,
                Mnemonic = model?.Mnemonic,
                IsDisabled = model?.IsDisabled ?? false
            };

        public static ChatLightDTO[] Create(IEnumerable<ChatView> models)
            => models.Select(Create).ToArray();
    }
}