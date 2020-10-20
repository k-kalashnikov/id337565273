using System.Threading;
using System.Threading.Tasks;
using SP.Messenger.Application.Messages.Command.CloseBotMessage;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;
using Xunit;

namespace SP.Messenger.Application.Test.Messages
{
    [Collection("QueryCollection")]
    public class CloseBotMessageTest
    {
        private readonly MessengerDbContext _context;

        public CloseBotMessageTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
        }
        
        [Theory]
        [InlineData(77519, 15000, "tester")]
        public async Task CloseBotMessageHandleTest(long messageId, dynamic value, string content)
        {
            var handler = new CloseBotMessageCommandHandler(_context);
            var command = CloseBotMessageCommand.Create(messageId, value, content);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.IsType<ProcessingResult<bool>>(result);
        }
    }
}