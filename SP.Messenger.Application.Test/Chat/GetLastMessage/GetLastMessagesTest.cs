using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetLastMessage;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.Chat.GetLastMessage
{
    public class GetLastMessagesTest : TestPopstgresBase
    {
        private readonly Mock<IOptions<Settings>> _options;
        public GetLastMessagesTest()
        {
            _options = new Mock<IOptions<Settings>>();
        }
        //[Fact]
        //public async Task GetLastMessagesHandleTestLife()
        //{
        //    var settings = new Settings
        //    {
        //        RMQClient = new RMQClient(),
        //        Logging = new Logging(),
        //        Templates = new Templates(),
        //        AllowedHosts = "*",
        //        ConnectionString = new ConnectionString(),
        //        FileServer = new FileServer
        //        {
        //            Storage = "http://localhost:5005/api/v1/messenger"
        //        }
        //    };

        //    _options.Setup(m => m.Value)
        //        .Returns(settings);

        //    await using var context = GetDbContext();
        //    var handler = new GetLastMessageQueryHandler(context, _options.Object);
        //    var command = GetLastMessageQuery.Create(Guid.Parse("e2403d09-cc8e-4885-b9a9-520e8d31d609"));
        //    var result = await handler.Handle(command, CancellationToken.None);
        //    result.ShouldBeOfType<ChatMessengerDTO[]>();
        //    Assert.NotNull(result);
        //}
    }
}