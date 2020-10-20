using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Domains.Views
{
    public class LastMessagesView
    {
        public long ChatId { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public int ChatTypeId { get; set; }
        public ChatType ChatType { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime CreateDate { get; set; }
        public long MessageId { get; set; }
        public long MessageTypeId { get; set; }
        public string Content { get; set; }
        public Guid DocumentId { get; set; }
        public string Mnemonic { get; set; }
        public bool Pined { get; set; }
        public static string View => "lastmessagesstockorderview";
    }
}