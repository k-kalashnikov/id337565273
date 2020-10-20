using System;

namespace SP.Messenger.Domains.Entities
{
    public class Message
    {
        public Message()
        {

        }
        public Message(long chatId, long accountId, bool pined, long messageTypeId, 
            string content, DateTime currentDate, long? recipientId = null)
        {
            ChatId = chatId;
            AccountId = accountId;
            Pined = pined;
            MessageTypeId = messageTypeId;
            Content = content;
            CreateDate = currentDate;
            Readed = false;
            ReadedDate = null;
            Modifed = false;
            ModifedDate = null;
            RecipientId = recipientId;
        }
        public long MessageId { get; set; }
        public DateTime CreateDate { get; set; }
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public long? RecipientId { get; set; }
        public Account Recipient { get; set; }
        public bool Pined { get; set; }
        public long MessageTypeId { get; set; }
        public MessageType MessageType { get; set; }
        public bool Readed { get; set; }
        public DateTime? ReadedDate { get; set; }
        public bool Modifed { get; set; }
        public DateTime? ModifedDate { get; set; }
        public string Content { get; set; } //ContentMessage
    }
}
