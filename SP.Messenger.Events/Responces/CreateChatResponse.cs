using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Consumers.Models
{
  public class CreateChatResponse
  {
    public CreateChatResponse(ChatInfoResponse chat)
    {
      Chat = chat;
    }

    public ChatInfoResponse Chat { get; set; }
    public static CreateChatResponse Create(ChatInfoResponse chat)
        => new CreateChatResponse(chat);
  }

  public class ChatInfoResponse
  {
    public string ChatName { get; set; }
    public long ChatId { get; set; }
    public Guid DocumentId { get; set; }
    public Guid? ParentDocumentId { get; set; }
    public string ChatTypeMnemonic { get; set; }
  }
}
