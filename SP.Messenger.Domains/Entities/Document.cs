using System;
using System.Collections.Generic;

namespace SP.Messenger.Domains.Entities
{
    public class Document
    {
        public Document()
        {
            Chats = new HashSet<ChatDocument>();
        }
        public Guid DocumentId { get; set; }
        public long DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public ICollection<ChatDocument> Chats { get; set; }
    }
}
