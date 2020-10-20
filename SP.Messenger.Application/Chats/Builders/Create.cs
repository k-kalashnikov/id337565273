using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Application.Chats.Builders
{
    public static class Create
    {
        public static Chat Chat(string name, int chatTypeId, ChatData data, bool isActive, long? parentChatId = null) 
            => new BuilderChat(name, chatTypeId, data, isActive, parentChatId).Build();
        public static Document Document(Guid documentId, long documentTypeId)
            => new BuilderDocument(documentId, documentTypeId).Build();
        public static Chat ContextChat(string name, int chatTypeId, ChatData data, bool isActive, long? parentChatId = null)
            => new BuilderContextChat(name, chatTypeId, data, isActive, parentChatId).Build();
    }
}
