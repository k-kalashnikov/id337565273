using System.ComponentModel.DataAnnotations;

namespace SP.Messenger.Domains.Entities
{
    public class MessageBot
    {
        [Key]
        public long MessageBotId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
    }
}
