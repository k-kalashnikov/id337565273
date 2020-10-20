using System.Collections.Generic;
using System.Linq;
using SP.Messenger.Domains.Entities;

namespace SP.Consumers.Models
{
    public class MessageTypeInsideDTO
    {
        public long MessageTypeId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }

        public static MessageTypeInsideDTO Create(MessageType model)
            => new MessageTypeInsideDTO
            {
                MessageTypeId = model?.MessageTypeId ?? 0,
                Name = model?.Name,
                IsDisabled = model?.IsDisabled ?? false
            };

        public static MessageTypeInsideDTO[] Create(IEnumerable<MessageType> models)
            => models.Select(x => Create(x)).ToArray();
    }
}