using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Queries.GetChatByDocumentId;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Application.Messages.Command.PurchaseUpdateStatusToOffersCollect;
using SP.Messenger.Persistence;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Hub.Service.Consumers.Messages.Commands.PurchaseUpdateStatusToOffers
{
  public class PurchaseUpdateStatusToOffersConsumer : IConsumer<SP.Market.EventBus.RMQ.Shared.Events.Purchase.PurchaseUpdateStatusToOffersCollectEvent>
  {
    private readonly IMediator Mediator;

    private readonly MessengerDbContext DbContext;

    public PurchaseUpdateStatusToOffersConsumer(IMediator _mediator, MessengerDbContext _dbContext)
    {
      Mediator = _mediator;
      DbContext = _dbContext;
    }


    public async Task Consume(ConsumeContext<SP.Market.EventBus.RMQ.Shared.Events.Purchase.PurchaseUpdateStatusToOffersCollectEvent> _eventContext)
    {
      Log.Information($"PurchaseUpdateStatusToOffersConsumer _eventContext {_eventContext.MessageId} {_eventContext.Message.DocumentId}");
      var command = PurchaseUpdateStatusToOffersCollectCommand.Create(_eventContext.Message.DocumentId, _eventContext.Message.Comment, _eventContext.Message.ResponsibleId);
      Log.Information($"PurchaseUpdateStatusToOffersConsumer {command.DocumentId} {command.ResponsibleId} {command.Comment}");
      await Mediator.Send(command, _eventContext.CancellationToken);
    }
  }
}
