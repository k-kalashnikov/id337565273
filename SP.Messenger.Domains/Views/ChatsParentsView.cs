using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Domains.Views
{
    public class ChatsParentsView
    {
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
        public long? ParentId { get; set; }
        public string DocumentId { get; set; }
        public static string View => "chatsparentsview";
    }
}