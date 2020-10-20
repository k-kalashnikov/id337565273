using System;

namespace SP.Messenger.Application.Chats.Queries.GetChatsPages.Filters
{
    public class ChatFilter
    {
        public DateTime? StartDay { get; set; }
        public DateTime? FinishDay { get; set; }
        public string Module { get; set; }
        public Guid? DocumentId { get; set; }
    }
}
