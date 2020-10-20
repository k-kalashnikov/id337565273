using Shouldly;
using SP.Messenger.Application.ChatTypeRoles.Models;
using SP.Messenger.Application.ChatTypeRoles.Queries;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.ChatTypeRoles
{
    [Collection("QueryCollection")]
    public class ChatTypeRolesTest
    {
        private readonly MessengerDbContext _context;
        public ChatTypeRolesTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
        }

        [Theory]
        [InlineData("module.market.pusrchase.chat.private")]
        public async Task GetChatTypeRolesByChatTypeMnemonicHandlerTest(string mnemonic)
        {
            var handler = new GetRolesQueryHandler(_context);
            var result = await handler.Handle(GetRolesQuery.Create(mnemonic), CancellationToken.None);
            result.ShouldBeOfType<ChatTypeRolesDto>();
        }
    }
}
