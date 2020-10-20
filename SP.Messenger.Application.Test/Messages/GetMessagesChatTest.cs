using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SP.Consumers.Models;
using SP.FileStorage.Client.Services;
using SP.Messenger.Application.Messages.Queries.GetMessagesChat;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Application.Voting.Models;
using SP.Messenger.Application.Voting.Queries;
using SP.Messenger.Common.Settings;
using SP.Messenger.Persistence;
using Xunit;

namespace SP.Messenger.Application.Test.Messages
{
  [Collection("QueryCollection")]
  public class GetMessagesChatTest
  {
    private readonly MessengerDbContext _context;
    private readonly Mock<IOptions<Settings>> _options;
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IFileStorageClientService> _fileStorage;

    public GetMessagesChatTest(QueryTestFixture fixture)
    {
      _context = fixture.Context;
      _options = new Mock<IOptions<Settings>>();
      _mediator = new Mock<IMediator>();
      _fileStorage = new Mock<IFileStorageClientService>();
    }

    [Theory]
    [InlineData(22221, 22941)]
    public async Task GetMessagesChatQueryTest(long _accountId, long _chatId)
    {
      var command = GetMessagesChatQuery.Create(_accountId, _chatId);
      var messages = await _mediator.Object.Send(command);
      messages.ShouldBeOfType<MessageClient[]>();
    }
  }
}
