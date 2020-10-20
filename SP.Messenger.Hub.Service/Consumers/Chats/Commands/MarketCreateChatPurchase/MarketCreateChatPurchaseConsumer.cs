using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using System;
using System.Linq;
using System.Threading.Tasks;
using MN = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.MarketCreateChatPurchase
{
  public class MarketCreateChatPurchaseConsumer : IConsumer<CreateMarketPurchaseChatRequest>
  {
    private readonly IMediator _mediator;

    public MarketCreateChatPurchaseConsumer(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CreateMarketPurchaseChatRequest> context)
    {
      Log.Information($"{nameof(MarketCreateChatPurchaseConsumer)} invoked");



      var request = context.Message;
      var command = CreateChatCommand.Create(request.PurchaseName,
              request.ChatTypeMnemonic,
              true,
              request.PurchaseId,
              request.DocumentTypeId,
              request.DocumentStatusMnemonic,
              (MN)Enum.Parse<MN>(request.Module),
              request.Accounts.Select(x => new AccountMessengerDTO
              {
                AccountId = x.AccountId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Login = x.Login,
                MiddleName = x.MiddleName
              }).ToArray(),
              request.ParentDocumentId,
              null);

      var resultCommand = await _mediator.Send(command, context.CancellationToken);
      var response = new ChatInfoResponse
      {
        ChatName = request.PurchaseName,
        ChatId = resultCommand.Result,
        DocumentId = request.PurchaseId,
        ParentDocumentId = null,
        ChatTypeMnemonic = request.ChatTypeMnemonic
      };
      await context.RespondAsync(CreateChatResponse.Create(response));
    }
  }
}
