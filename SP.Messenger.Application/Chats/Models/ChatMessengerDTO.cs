using SP.Consumers.Models;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Common.Settings;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Application.Chats.Models
{
  public class ChatMessengerDTO
  {
    public long ChatId { get; set; }
    public Guid DocumentId { get; set; }
    public long? ParentChatId { get; set; }
    public string Name { get; set; }
    public string Mnemonic { get; set; }
    public AccountMessengerDTO Account { get; set; }
    public ChatTypeDTO ChatType { get; set; }
    public ICollection<MessageClient> Messages { get; private set; }
    public bool IsDisabled { get; set; }
    public DateTime CreateAt { get; set; }
    public Metadata Meta { get; set; }

    //Chat
    public static ChatMessengerDTO Create(Chat model)
    => new ChatMessengerDTO
    {
      ChatId = model.ChatId,
      ParentChatId = model.ParentId,
      Name = model.Name,
      ChatType = ChatTypeDTO.Create(model.ChatType),
      IsDisabled = model.IsDisabled,
      CreateAt = model.CreateAt
    };
    public static ChatMessengerDTO[] Create(IEnumerable<Chat> models)
       => models.Select(Create).ToArray();

    //SpecificationView
    public static ChatMessengerDTO Create(SpecificationView view)
    => new ChatMessengerDTO
    {
      ChatId = view.ChatId,
      Name = view.Name,
      IsDisabled = view.IsDisabled
    };

    public static ChatMessengerDTO[] Create(IEnumerable<SpecificationView> models)
        => models.Select(Create).ToArray();

    //LastMessagesView
    public static ChatMessengerDTO Create(LastMessagesView view, string host)
    {
      var msgDb = view.Content?.FromJson<Consumers.Models.ContentMessage>();
      Consumers.Models.MessageFile file = null;
      if (!string.IsNullOrWhiteSpace(msgDb?.File.Url))
      {
        file = msgDb.File;
        file.Url = $"{host}{msgDb.File.Url}";
      }

      var messageType = Enum.Parse<MessageTypeClient>(view.MessageTypeId.ToString());

      var chat = new ChatMessengerDTO
      {
        ChatId = view.ChatId,
        ParentChatId = view.ParentId,
        Name = view.Name,
        Mnemonic = view.Mnemonic,
        DocumentId = view.DocumentId,
        ChatType = ChatTypeDTO.Create(view.ChatType),
        CreateAt = view.CreateDate,
        Messages = view.Content is null ? new MessageClient[] { } : new[]
          {
                    new MessageClient
                    {
                        MessageId = view.MessageId,
                        Author = Author.Create(view.Account?.AccountId??0, view.Account?.Login,
                            view.Account?.FirstName, view.Account?.LastName, view.Account?.MiddleName),

                        Date = view.CreateDate,
                        ChatId = view.ChatId,
                        DocumentId = view.DocumentId,
                        MessageType = messageType,
                        ModuleName = ModuleName.Bid,
                        ChatTypeMnemonic = view.ChatType?.Mnemonic,
                        Content = string.IsNullOrEmpty(msgDb?.Content) ? msgDb?.CommandClient.DisplayName : msgDb.Content,
                        File = file,
                        Readed = false,
                        Pined = view.Pined,
                        ButtonCommands = new ButtonCommand[]{},
                        Commands = new []{ msgDb?.CommandClient }
                    }
                },
        Account = AccountMessengerDTO.Create(view.Account)
      };
      return chat;
    }

    public static ChatMessengerDTO[] Create(IEnumerable<LastMessagesView> models, string host)
        => models.Select(x => Create(x, host)).ToArray();

    public static ChatMessengerDTO Create(ChatView view)
        => new ChatMessengerDTO
        {
          ChatId = view.ChatId,
          ParentChatId = view.ParentId,
          Name = view.Name,
          IsDisabled = view.IsDisabled,
          DocumentId = Guid.Parse(view.DocumentId),
          Mnemonic = view.Mnemonic
        };
    public static ChatMessengerDTO[] Create(IEnumerable<ChatView> views)
        => views.Select(Create).ToArray();
  }

  public class Metadata
  {
      public string ProtocolNumber { get; set; }
      public DateTime ProtocolStartDate { get; set; }
      public bool IsClosed { get; set; }
  }
}

