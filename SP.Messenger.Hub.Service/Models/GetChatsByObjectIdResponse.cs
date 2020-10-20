using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChatsPages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using J = Newtonsoft.Json.JsonPropertyAttribute;

namespace SP.Consumers.Models
{
  public class GetChatsByObjectIdResponse
  {
    [J("chatId")]
    public long ChatId { get; set; }

    [J("parentId")]
    public long? ParentId { get; set; }

    [J("documentId")]
    public Guid DocumentId { get; set; }

    [J("name")]
    public string Name { get; set; }

    [J("mnemonic")]
    public string Mnemonic { get; set; }

    [J("account")]
    public AccountMessengerDTO Account { get; set; }

    [J("chatType")]
    public ChatTypeDTO ChatType { get; set; }

    [J("messages")]
    public ICollection<MessageClient> Messages { get; private set; }

    [J("childs")]
    public ICollection<GetChatsByObjectIdResponse> Childs { get; set; }

    [J("isDisabled")]
    public bool IsDisabled { get; set; }
    
    [J("meta")]
    public Metadata Meta { get; set; }


    public static GetChatsByObjectIdResponse Create(ChatMessengerDTO model)
    {
      var chat = new GetChatsByObjectIdResponse
      {
        ChatId = model.ChatId,
        ParentId = model.ParentChatId,
        DocumentId = model.DocumentId,
        Name = model.Name,
        Mnemonic = model.Mnemonic,
        ChatType = model.ChatType,
        Messages = model.Messages,
        Account = model.Account,
        Meta = model.Meta
      };
      return chat;
    }

    public static GetChatsByObjectIdResponse[] Create(ChatMessengerDTO[] models)
    {
      return models.Select(Create).ToArray();
    }
  }
}
