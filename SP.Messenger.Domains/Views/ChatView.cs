using System;

namespace SP.Messenger.Domains.Views
{
    public class ChatView
    {
        public long ChatId { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
        public string Mnemonic { get; set; }
        public string DocumentId { get; set; }
        public string ParentDocumentId { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        public long? ContractorId { get; set; }
        public string Module { get; set; }
        public static string View => "chatview";
    }
}
