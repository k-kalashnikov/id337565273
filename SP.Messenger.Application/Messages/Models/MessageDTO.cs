using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Domains.Entities;
using System;
using System.Linq;

namespace SP.Messenger.Application.Messages.Models
{
    public class MessageDTO
    {
        public long MessageId { get; set; }
        public DateTime CreateDate { get; set; }
        public long ChatId { get; set; }
        public ChatMessengerDTO Chat { get; set; }
        public long AccountId { get; set; }
        public AccountMessengerDTO Account { get; set; }
        public long MessageTypeId { get; set; }
        public MessageTypeDTO MessageType { get; set; }
        public Consumers.Models.ContentMessage Content { get; set; }

        public static MessageDTO Create(Message model)
            => new MessageDTO
            {
                MessageId = model.MessageId,
                CreateDate = model.CreateDate,
                AccountId = model.AccountId,
                Account = AccountMessengerDTO.Create(model.Account),
                ChatId = model.ChatId,
                Chat = ChatMessengerDTO.Create(model.Chat),
                MessageTypeId = model.MessageTypeId,
                MessageType = MessageTypeDTO.Create(model.MessageType),
                Content = model.Content.FromJson<Consumers.Models.ContentMessage>()
            };

        public static MessageDTO[] Create(Message[] models)
            => models.Select(x => Create(x)).ToArray();

        public static MessageDTO Build(long messageId, DateTime createDate, 
                        long chatId, long messageTypeId, long accountId, string content)
        {
            return new MessageDTO
            {
                MessageId = messageId,
                CreateDate = createDate,
                ChatId = chatId,
                MessageTypeId = messageTypeId,
                AccountId = accountId,
                Content = content.FromJson<Consumers.Models.ContentMessage>()
            };
        }
    }
}
