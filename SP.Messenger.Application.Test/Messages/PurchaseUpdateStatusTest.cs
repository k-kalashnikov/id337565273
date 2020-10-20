using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Messages.Command.PurchaseUpdateStatusToOffersCollect;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Persistence;
using Xunit;
using SP.Messenger.Common.Settings;
using Microsoft.AspNetCore.Http;
using SP.FileStorage.Client.Services;
using SP.Messenger.Domains.Views;
using SP.Messenger.Application.Chats.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SP.Messenger.Application.Test.Messages
{
  [Collection("QueryCollection")]
  public class PurchaseUpdateStatusTest 
  {
    private readonly Mock<IMediator> Mediator;
    private readonly Mock<IOptions<Settings>> Options;
    private readonly IHttpContextAccessor ContextAccessorMock;
    private readonly Mock<IFileStorageClientService> FileStorage;
    private readonly MessengerDbContext Context;

    public PurchaseUpdateStatusTest(QueryTestFixture fixture)
    {
      Mediator = new Mock<IMediator>();
      Options = new Mock<IOptions<Settings>>();
      ContextAccessorMock = fixture.ContextAccessor;
      FileStorage = new Mock<IFileStorageClientService>();
      Context = fixture.Context;
    }

    [Theory]
    [InlineData("d7f7c646-dcf8-4e4b-9d3d-17fb5072d5fa", "PurchaseUpdateStatusToOffersCollectTest comment", 22221)]
    public async Task PurchaseUpdateStatusToOffersCollectTest(Guid _documentId, string _comment, long _responsibleId)
    {
      //актуальные данные для запроса чатов
      //var getChatQuery = GetChatsQuery.Create(_responsibleId, _documentId);
      //var handlerGetChats = new GetChatsQueryHandler(Context, Options.Object, ContextAccessorMock, FileStorage.Object);
      //var resultGetChats = await handlerGetChats.Handle(getChatQuery, CancellationToken.None);
      var resultGetChats = new List<ChatMessengerDTO>()
      {
        new ChatMessengerDTO()
        {
          ChatId = 22756,
          Name = "ТестПоставка2 (ДЛЯ РАЗРАБОТЧИКОВ)",
          DocumentId = Guid.Parse("c28df046-3b18-4644-bd53-8b871de4851d"),
          IsDisabled = false,
          Mnemonic = "module.market.pusrchase.chat.private",
          ParentChatId = 22755
        },
        new ChatMessengerDTO()
        {
          ChatId = 22757,
          Name = "ТестПоставка (ДЛЯ РАЗРАБОТЧИКОВ)",
          DocumentId = Guid.Parse("37ded1ad-bab5-4a7f-834f-e05ce0b3b1bf"),
          IsDisabled = false,
          Mnemonic = "module.market.pusrchase.chat.private",
          ParentChatId = 22755
        },
      }.ToArray();

      Mediator
        .Setup(m => m.Send(It.IsAny<GetChatsQuery>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(resultGetChats);




      var commandPurchaseStatus = PurchaseUpdateStatusToOffersCollectCommand.Create(_documentId, _comment, _responsibleId);

      Mediator
        .Setup(m => m.Send(It.IsAny<PurchaseUpdateStatusToOffersCollectCommand>(), It.IsAny<CancellationToken>()));

      using (var context = GetDbContext())
      {
        var handlerPurchaseStatus = new PurchaseUpdateStatusToOffersCollectCommandHandler(Mediator.Object, context);
        var resultPurchaseStatus = await handlerPurchaseStatus.Handle(commandPurchaseStatus, CancellationToken.None);
        resultPurchaseStatus.Result.ShouldBeOfType<bool>();
        Assert.True(resultPurchaseStatus.Result);
      }
    }


    private MessengerDbContext GetDbContext(bool initialize = false)
    {
      var config = new ConfigurationBuilder()
         .AddJsonFile("appsettings.json")
         .Build();

      var connectionString = config["ConnectionStrings:MessengerDatabase"];
      var builder = new DbContextOptionsBuilder<MessengerDbContext>();
      builder.UseNpgsql(connectionString);

      var context = new MessengerDbContext(builder.Options);

      if (initialize)
      {
        MessengerInitializer.InitializeAsync(context);
      }

      return context;
    }
  }
}
