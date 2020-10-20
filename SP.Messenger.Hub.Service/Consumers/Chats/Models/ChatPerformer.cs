using System;
using System.Collections;

namespace SP.Consumers.Models
{
    public class ChatPerformer
    {
        public long AuthorId { get; set; }
        public long PerformerId  { get; set; }
        public string Name { get; set; }
        public string Module { get; set; }
        public Guid DocumentId { get; set; }
        public int DocumentTypeId { get; set; }
        public Guid ParentDocumentId { get; set; }
        public string ChatTypeMnemonic { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        public long[] Accounts { get; set; }
    }
}