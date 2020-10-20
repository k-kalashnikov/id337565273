using System;
using System.Collections.Generic;

namespace SP.Messenger.Domains.Entities
{
  public class Chat
  {
    public Chat()
    {
      Accounts = new HashSet<AccountChat>();
      Messages = new HashSet<Message>();
      Documents = new HashSet<ChatDocument>();
    }

    public long ChatId { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; }
    public bool IsDisabled { get; set; }
    public int ChatTypeId { get; set; }
    public ChatType ChatType { get; set; }
    public string Data { get; set; } //ChatData
    public DateTime CreateAt { get; set; }
    public ICollection<AccountChat> Accounts { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<ChatDocument> Documents { get; set; }
  }
}
