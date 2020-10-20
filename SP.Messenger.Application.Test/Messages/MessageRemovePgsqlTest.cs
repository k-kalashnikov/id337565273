using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using SP.Messenger.Application.Messages.Command.Remove;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Implementations;
using Xunit;

namespace SP.Messenger.Application.Test.Messages
{
    public class MessageRemovePgsqlTest: TestPopstgresBase
    {
        [Fact]
        public async Task GetLastMessagesHandleTestLife()
        {
            using (var context = GetDbContext())
            {
                long accountId = 1;
                long messageId = 607;
                var handler = new RemoveMessageCommandHandler(context);
                var result = await handler.Handle(RemoveMessageCommand.Create(accountId, messageId), CancellationToken.None);
                result.ShouldBeOfType<ProcessingResult<bool>>();
            }
        }
    }
}