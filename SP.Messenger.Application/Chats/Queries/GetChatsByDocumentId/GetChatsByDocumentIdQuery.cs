using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SP.Messenger.Application.Chats.Models;

namespace SP.Messenger.Application.Chats.Queries.GetChatsByDocumentId
{
  public class GetChatsByDocumentIdQuery : IRequest<ChatMessengerDTO[]>
  {
    public Guid DocumentId { get; set; }

    public static GetChatsByDocumentIdQuery Create(Guid _documentId)
    {
      return new GetChatsByDocumentIdQuery()
      {
        DocumentId = _documentId
      };
    }
  }
}
