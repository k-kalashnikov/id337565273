using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events.Need;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Chats.Queries.GetChatsByDocumentId;
using SP.Messenger.Application.Messages.Command;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Hub.Service.Consumers.Messages.Commands.NeedRemove
{
  //писать другой косьюмер с таким же кодом только разным мессаджем - неправильно. Пусть текст сообщения присылают с маркета
  public class NeedRemoveConsumer : IConsumer<RemoveNeedFromPurchaseEvent>
  {
    private readonly IMediator _mediator;

    private readonly ICurrentUserService _userService;

    public NeedRemoveConsumer(IMediator mediator,
      ICurrentUserService userService)
    {
      _mediator = mediator;
      _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    }

    public async Task Consume(ConsumeContext<RemoveNeedFromPurchaseEvent> context)
    {
      var message = context.Message;
      var findChatsQuery = GetChatsByDocumentIdQuery.Create(message.PurchaseId);
      var chats = await _mediator.Send(findChatsQuery, context.CancellationToken);

      if ((chats == null) || (chats.Length == 0)) {
        Log.Error($"{nameof(NeedRemoveConsumer)} - Chats not found");
        return;
      }

      var chat = chats.FirstOrDefault(m => m.ParentChatId == null);

      if (chat == null) {
        Log.Error($"{nameof(NeedRemoveConsumer)} - Chat not found");
        return;
      }

      var author = _userService.GetCurrentUser();

      if (author == null)
      {
        Log.Error($"{nameof(NeedRemoveConsumer)} - Current User is null");
        return;
      }

      var messageClient = new MessageClient
      {
        Commands = Array.Empty<CommandClient>(),
        Content = $"{author?.LastName ?? ""} {author?.FirstName ?? ""} {author?.MiddleName ?? ""}  {message.Comment}",
        Date = DateTime.UtcNow,
        DocumentId = message.PurchaseId,
        File = null,
        ButtonCommands = Array.Empty<ButtonCommand>(),
        ModuleName = ModuleName.Market,
        ChatTypeMnemonic = chat.Mnemonic,
        ChatId = chat.ChatId,
        MessageId = 1,
        Author = new Author
        (
            author.Id,
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

      var sendMessage = messenger.Create.Message
      (
          chatId: chat.ChatId,
          accountId: author.Id,
          messageTypeId: 5,
          content: messageClient
      );

      var saveMessageCommand = SaveMessageCommand.Create(sendMessage, author.Login);
      var resultSaveMessageCommand = await _mediator.Send(saveMessageCommand, context.CancellationToken);
    }
  }
}
