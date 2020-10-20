using SP.Messenger.Domains.Entities;
using System;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;

namespace SP.Messenger.Application.Messages.Builders
{
    public static class Create
    {
        public static Message Message(long chatId, long accountId, long messageTypeId, MessageClient content, long? recipientId = null)
            => new BuilderMessage(chatId, accountId, messageTypeId, content, recipientId).Build();
    }
}
