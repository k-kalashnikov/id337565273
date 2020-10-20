using SP.Messenger.Domains.Entities;
using System.Collections.Generic;
using System.Linq;

namespace SP.Consumers.Models
{
    public class MessageTypeDTO
    {
        public long MessageTypeId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }

        public static MessageTypeDTO Create(MessageType model)
        => new MessageTypeDTO
        {
            MessageTypeId = model.MessageTypeId,
            Name = model.Name,
            IsDisabled = model.IsDisabled
        };

        public static MessageTypeDTO[] Create(IEnumerable<MessageType> models)
        => models.Select(x => Create(x)).ToArray();
    }
}
