using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Commands.CreateVoteAutoChat;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.MarketCreateChatVote
{
  public class MarketCreateChatVoteConsumer : IConsumer<MarketCreateChatVoteRequest>
  {
    private readonly IMediator _mediator;

    public MarketCreateChatVoteConsumer(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<MarketCreateChatVoteRequest> context)
    {
      var data = context.Message;

      var variants = new List<ResponseVariantChatCommand>();
      foreach (var item in data.ResponseVariants)
      {
        variants.Add(new ResponseVariantChatCommand(
            item.VariantName,
            item.DecisionId,
            contractors: item.Contractors.Select(x => new Application.Chats.Commands.CreateVoteAutoChat.Contractor
            {
              ContractorId = x.ContractorId,
              ContractorName = x.ContractorName,
              PriceOffer = x.PriceOffer,
              DeviationBestPrice = x.DeviationBestPrice,
              Term = x.Term,
              TermDeviation = x.TermDeviation,
              DefermentPayment = x.DefermentPayment,
              DefermentDeviation = x.DefermentDeviation,
              PercentDifferentByPurchase = x.PercentDifferentByPurchase,
              PercentDifferentByBestContractorOffer = x.PercentDifferentByBestContractorOffer,
              TermLimit = x.TermLimit
            })));
      }

      var command = CreateVoteAutoChatCommand.Create(
          data.Name,
          data.DocumentId,
          data.ParentDocumentId,
          variants,
          data.Accounts,
          data.ProtocolNumber,
          data.ProtocolStartDate);
      var result = await _mediator.Send(command, context.CancellationToken);

      await context.RespondAsync(MarketCreateChatVoteResponse.Create(result, null));
    }
  }
}
