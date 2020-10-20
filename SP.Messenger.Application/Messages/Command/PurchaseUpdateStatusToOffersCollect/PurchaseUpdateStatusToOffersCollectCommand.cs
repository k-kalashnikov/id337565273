using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.PurchaseUpdateStatusToOffersCollect
{
  public class PurchaseUpdateStatusToOffersCollectCommand : IRequest<ProcessingResult<bool>>
  {
    public Guid DocumentId { get; set; }
    public string Comment { get; set; }
    public long ResponsibleId { get; set; }

    public PurchaseUpdateStatusToOffersCollectCommand(Guid _documentId, string _comment, long _responsibleId)
    {
      DocumentId = _documentId;
      Comment = _comment;
      ResponsibleId = _responsibleId;
    }

    public static PurchaseUpdateStatusToOffersCollectCommand Create(Guid _documentId, string _comment, long _responsibleId)
      => new PurchaseUpdateStatusToOffersCollectCommand(_documentId, _comment, _responsibleId);
  }
}
