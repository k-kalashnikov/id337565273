using System;

namespace SP.Messenger.Domains.Entities
{
    public class ChatDocument
    {
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
        public Guid DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
