using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Application.Messages.Models
{
    public class MessageTypeDTO
    {
        public long MessageTypeId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }

        public static MessageTypeDTO Create(Domains.Entities.MessageType model)
        => new MessageTypeDTO
        {
            MessageTypeId = model.MessageTypeId,
            Name = model.Name,
            IsDisabled = model.IsDisabled
        };

        public static MessageTypeDTO[] Create(IEnumerable<Domains.Entities.MessageType> models)
        => models.Select(x => Create(x)).ToArray();
    }
}
