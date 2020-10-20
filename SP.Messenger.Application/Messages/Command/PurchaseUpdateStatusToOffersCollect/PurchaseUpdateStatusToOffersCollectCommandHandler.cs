using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit.Conductor.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Application.Messages.Command.PurchaseUpdateStatusToOffersCollect
{
  public class PurchaseUpdateStatusToOffersCollectCommandHandler : IRequestHandler<PurchaseUpdateStatusToOffersCollectCommand, ProcessingResult<bool>>
  {

    private readonly IMediator Mediator;

    private readonly MessengerDbContext DbContext;

    public PurchaseUpdateStatusToOffersCollectCommandHandler(IMediator _mediator, MessengerDbContext _dbContext)
    {
      Mediator = _mediator;
      DbContext = _dbContext;
    }

    public async Task<ProcessingResult<bool>> Handle(PurchaseUpdateStatusToOffersCollectCommand request, CancellationToken cancellationToken)
    {
      var getChatByDocumentQuery = GetChatsQuery.Create(request.ResponsibleId, request.DocumentId);
      var resultGetChatByDocumentQuery = await Mediator.Send(getChatByDocumentQuery, cancellationToken);

      Log.Information($"PurchaseUpdateStatusToOffersCollectCommandHandler chats = ${String.Join(',', resultGetChatByDocumentQuery.Select(m => $"{m.ChatId}"))}");


      var author = await DbContext.Accounts.FirstOrDefaultAsync(m => m.AccountId == request.ResponsibleId, cancellationToken) ?? new Domains.Entities.Account()
      {
        AccountId = 1,
        FirstName = "Супер",
        Login = "stec.superuser@mail.ru",
        MiddleName = "",
        LastName = "Пользователь"
      };

      Log.Information($"PurchaseUpdateStatusToOffersCollectCommandHandler authorID = ${author.AccountId}");

      foreach (var item in resultGetChatByDocumentQuery)
      {
        var messageClient = new MessageClient
        {
          Commands = Array.Empty<CommandClient>(),
          Content = $"{author?.LastName ?? ""} {author?.FirstName ?? ""} {author?.MiddleName ?? ""} направил закупку на обсуждение условий и цен поставки. Необходимо предложить более выгодные условия. Комментарий: {request.Comment}",
          Date = DateTime.UtcNow,
          DocumentId = item.DocumentId,
          File = null,
          ButtonCommands = Array.Empty<ButtonCommand>(),
          ModuleName = ModuleName.Market,
          ChatTypeMnemonic = item.Mnemonic,
          ChatId = item.ChatId,
          MessageId = 1,
          Author = new Author
                (
                    author.AccountId,
                    author.Login,
                    author.FirstName,
                    author.LastName,
                    author.MiddleName
                ),
          MessageType = MessageTypeClient.User,
          Edited = false,
          Pined = false,
          Readed = false,
          VotingClient = null
        };

        var message = messenger.Create.Message
        (
            chatId: item.ChatId,
            accountId: request.ResponsibleId,
            messageTypeId: 5,
            content: messageClient
        );
        var saveMessageCommand = SaveMessageCommand.Create(message, author.Login);
        var resultSaveMessageCommand = await Mediator.Send(saveMessageCommand, cancellationToken);
      }
      return new ProcessingResult<bool>(true);
    }
  }
}
