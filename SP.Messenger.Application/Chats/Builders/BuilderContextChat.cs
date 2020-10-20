using MongoDB.Bson;
using SP.Messenger.Domains.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Chats.Builders
{
    class BuilderContextChat
    {
        private Chat chat;

        public BuilderContextChat(string name, int chatTypeId, ChatData data, bool isActive = true, long? parentChatId = null)
        {
            data.DocumentId = data.DocumentId.ToLower();
            data.DocumentStatusMnemonic = data.DocumentStatusMnemonic.ToLower();
            chat = new Chat();
            chat.ParentId = parentChatId;
            chat.Name = name;
            chat.ChatTypeId = chatTypeId;
            chat.Data = data.ToJson();
            chat.IsDisabled = !isActive;
        }

        public Chat Build() => chat;
    }
}
