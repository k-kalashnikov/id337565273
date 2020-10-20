using System;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Chats.Builders
{
  public class BuilderChat
  {
    private Chat chat;
    public BuilderChat(string name, int chatTypeId, ChatData data, bool isActive = true, long? parentChatId = null)
    {
      data.DocumentId = data.DocumentId.ToLower();
      chat = new Chat();
      chat.ParentId = parentChatId;
      chat.Name = name;
      chat.ChatTypeId = chatTypeId;
      chat.Data = data.ToJson();
      chat.IsDisabled = !isActive;
      chat.CreateAt = DateTime.Now;
    }
    public Chat Build() => chat;
  }
}
