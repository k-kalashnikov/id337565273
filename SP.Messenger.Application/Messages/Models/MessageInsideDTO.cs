using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Voting.Queries;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Common.Helpers;
using SP.Messenger.Domains.Entities;

namespace SP.Consumers.Models
{
  public class MessageInsideDTO
  {
    public long MessageId { get; set; }
    public DateTime CreateDate { get; set; }
    public long ChatId { get; set; }
    public ChatMessengerDTO Chat { get; set; }
    public long AccountId { get; set; }
    public AccountMessengerDTO Account { get; set; }
    public long MessageTypeId { get; set; }
    public MessageTypeInsideDTO MessageType { get; set; }
    public ContentMessage Content { get; set; }

    public static MessageInsideDTO Create(Message model, string host, IMediator mediator)
    {
      MessageInsideDTO result = null;

      var content = model.Content.FromJson<ContentMessage>();
      if (model.MessageType.MessageTypeId == 3) //bot
      {
        content.Content = content.CommandClient.DisplayName;
      }

      if (!string.IsNullOrWhiteSpace(content?.File.Url))
      {
        content.File.Url = $"{host}{content.File.Url}";
      }

      result = new MessageInsideDTO
      {
        MessageId = model.MessageId,
        CreateDate = model.CreateDate,
        AccountId = model.AccountId,
        Account = AccountMessengerDTO.Create(model.Account),
        ChatId = model.ChatId,
        Chat = ChatMessengerDTO.Create(model.Chat),
        MessageTypeId = model.MessageTypeId,
        MessageType = MessageTypeInsideDTO.Create(model.MessageType),
        Content = content
      };

      return result;
    }

    public static MessageInsideDTO[] Create(Message[] models, string host, IMediator mediator)
        => models.Select(x => Create(x, host, mediator)).ToArray();

    public static MessageInsideDTO Build(long messageId, DateTime createDate,
        long chatId, long messageTypeId, long accountId, string email, string firstName, string lastName,
        string middleName, string content)
    {
      return new MessageInsideDTO
      {
        Account = new AccountMessengerDTO
        {
          AccountId = accountId,
          Login = email,
          FirstName = firstName,
          LastName = lastName,
          MiddleName = middleName
        },
        MessageId = messageId,
        CreateDate = createDate,
        ChatId = chatId,
        MessageTypeId = messageTypeId,
        AccountId = accountId,
        Content = content.FromJson<ContentMessage>()
      };
    }


    public static MessageClient[] Create(MessageInsideDTO[] messages, string host, IMediator mediator)
    {
      return messages?.Select(x =>
          new MessageClient
          {
            Author = Author.Create(x?.Account?.AccountId ?? 0, x?.Account?.Login,
                  x?.Account?.FirstName, x?.Account?.LastName, x?.Account?.MiddleName),

            MessageId = x?.MessageId ?? 0,
            Date = x?.CreateDate ?? DateTime.MinValue,
            Commands = new[]
              {
                        new CommandClient
                        {
                            Command = x?.Content?.CommandClient?.Command,
                            Open =  Enum.Parse<AvailabilityCommand>(x?.Content?.CommandClient?.Open.ToString()), //AvailabilityCommand.Closed,
                            DisplayName = x?.Content?.CommandClient?.DisplayName,
                            BotMessageType =  x?.Content?.CommandClient?.BotMessageType,
                            Value = x?.Content?.CommandClient?.Value
                        }
              },
            ButtonCommands = Array.Empty<ButtonCommand>(),
            ChatId = x?.ChatId ?? 0,
            ChatTypeMnemonic = x?.Content?.Tags?.ChatTypeMnemonic,
            DocumentId = x?.Content?.Tags?.DocumentId ?? Guid.Empty,
            File = new MessageFile
            {
              Extension = x?.Content?.File?.Extension,
              Filename = x?.Content?.File?.Filename,
              Lenght = x?.Content?.File?.Lenght ?? 0,
              Url = x?.Content?.File?.Url,
              ContentType = x?.Content?.File?.ContentType
            },
            Content = x?.Content?.Content,
            Readed = false,
            MessageType = EnumHelper.ConvertFromString<MessageTypeClient>(x?.MessageType.Name.ToString())

          }).ToArray();
    }

    public static async Task<MessageClient[]> CreateFromMessage(Message[] messages, string host, IMediator mediator)
    {
      var messagesClient = new List<MessageClient>();

      foreach (var item in messages)
      {
        var content = item.Content.FromJson<ContentMessage>();
        if (item.MessageType.MessageTypeId == 3 || item.MessageType.MessageTypeId == 4) //bot or system
          content.Content = content.CommandClient.DisplayName;

        if (!string.IsNullOrWhiteSpace(content?.File.Url))
          content.File.Url = $"{host}{content.File.Url}";

        messagesClient.Add(new MessageClient
        {
          Author = Author.Create(item.Account?.AccountId ?? 0, item.Account?.Login,
                item.Account?.FirstName, item.Account?.LastName, item.Account?.MiddleName),

          MessageId = item.MessageId,
          Date = item.CreateDate,
          Commands = new[]
            {
                        new CommandClient
                        {
                            Command = content?.CommandClient?.Command,
                            Open =  Enum.Parse<AvailabilityCommand>(content?.CommandClient?.Open.ToString()),
                            DisplayName = content?.CommandClient?.DisplayName,
                            BotMessageType =  content?.CommandClient?.BotMessageType,
                            Value = content?.CommandClient?.Value
                        }
                    },
          ButtonCommands = Array.Empty<ButtonCommand>(),
          ChatId = item.ChatId,
          ChatTypeMnemonic = content?.Tags?.ChatTypeMnemonic,
          DocumentId = content?.Tags?.DocumentId ?? Guid.Empty,
          File = new MessageFile
          {
            Extension = content?.File?.Extension,
            Filename = content?.File?.Filename,
            Lenght = content?.File?.Lenght ?? 0,
            Url = content?.File?.Url,
            ContentType = content?.File?.ContentType
          },
          Content = content?.Content,
          Edited = item.Modifed,
          Pined = item.Pined,
          Readed = false,
          ModuleName = (SP.Consumers.Models.ModuleName)Enum.Parse(typeof(SP.Consumers.Models.ModuleName), content.Tags.ModuleName.ToString()),
          MessageType = EnumHelper.ConvertFromString<MessageTypeClient>(item.MessageType.Name),
          VotingClient = await VotingClientBuilder(content.VotingContent?.VotingId, mediator)
        });
      }

      return messagesClient.ToArray();
    }
    public static async Task<MessageClient> CreateFromMessage(Message message, string host, IMediator mediator)
    {
      var content = message.Content.FromJson<ContentMessage>();
      if (message.MessageTypeId == 3) //bot
        content.Content = content.CommandClient.DisplayName;

      if (!string.IsNullOrWhiteSpace(content?.File.Url))
        content.File.Url = $"{host}{content.File.Url}";

      var messageClinet = new MessageClient
      {
        Author = Author.Create(message.Account?.AccountId ?? 0, message.Account?.Login,
              message.Account?.FirstName, message.Account?.LastName, message.Account?.MiddleName),

        MessageId = message.MessageId,
        Date = message.CreateDate,
        Commands = new[]
          {
                    new CommandClient
                    {
                        Command = content?.CommandClient?.Command,
                        Open = Enum.Parse<AvailabilityCommand>(content?.CommandClient?.Open.ToString()),
                        DisplayName = content?.CommandClient?.DisplayName,
                        BotMessageType = content?.CommandClient?.BotMessageType,
                        Value = content?.CommandClient?.Value
                    }
                },
        ButtonCommands = Array.Empty<ButtonCommand>(),
        ChatId = message.ChatId,
        ChatTypeMnemonic = content?.Tags?.ChatTypeMnemonic,
        DocumentId = content?.Tags?.DocumentId ?? Guid.Empty,
        File = new MessageFile
        {
          Extension = content?.File?.Extension,
          Filename = content?.File?.Filename,
          Lenght = content?.File?.Lenght ?? 0,
          Url = content?.File?.Url,
          ContentType = content?.File?.ContentType
        },
        Content = content?.Content,
        Readed = false,
        Pined = message.Pined,
        Edited = message.Modifed,
        ModuleName = (SP.Consumers.Models.ModuleName)Enum.Parse(typeof(SP.Consumers.Models.ModuleName), content.Tags.ModuleName.ToString()),
        MessageType = EnumHelper.ConvertFromString<MessageTypeClient>(message.MessageType.Name),
        VotingClient = await VotingClientBuilder(content.VotingContent?.VotingId, mediator)
      };

      return messageClinet;
    }

    private static async Task<VotingClient> VotingClientBuilder(Guid? votingId, IMediator mediator)
    {
      if (votingId is null)
        return null;

      var voting = await mediator.Send(GetVotingByIdQuery.Create(votingId.Value));
      if (voting is null)
        return null;

      var votingClient = new VotingClient
      {
        Name = voting.Name,
        VotingId = voting.Id,
        IsClosed = voting.IsClosed,
        AccountCount = voting.VotedCollection.Count(),
        VotingObjects = voting.ResponseVariants.Select(x => new VotingObject
        {
          VotingObjectId = x.Id,
          Name = x.Name,
          Contractors = x.VotingContractor.Select(vc => new VotingContractor
          {
            ContractorId = vc.ContractorId,
            ContractorName = vc.ContractorName,
            PriceOffer = vc.PriceOffer,
            DeviationBestPrice = vc.DeviationBestPrice,
            Term = vc.Term,
            TermDeviation = vc.TermDeviation,
            DefermentPayment = vc.DefermentPayment,
            DefermentDeviation = vc.DefermentDeviation,
            PercentDifferentByPurchase = vc.PercentDifferentByPurchase,
            PercentDifferentByBestContractorOffer = vc.PercentDifferentByBestContractorOffer,
            TermLimit = vc.TermLimit
          }),
          Accounts = voting.VotedCollection
            .Select(c => new VotedAccounts
            {
              AccountId = c.Voted,
              Email = c.Email,
              FirstName = c.FirstName,
              LastName = c.LastName,
              MiddleName = c.MiddleName,
              IsLike = c.IsLike
            })
            .ToArray()
        })
      };

      return votingClient;
    }
  }
}
