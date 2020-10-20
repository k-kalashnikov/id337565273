using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Settings;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.Chat.Queries.GetChats
{
    [Collection("QueryCollection")]
    public class GetChatsQueryHandlerTest
    {
        private readonly MessengerDbContext _context;
        private readonly Mock<IOptions<Settings>> _options;
        private readonly IHttpContextAccessor _contextAccessorMock;
        public GetChatsQueryHandlerTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _options = new Mock<IOptions<Settings>>();
            _contextAccessorMock = fixture.ContextAccessor;
        }

        //[Theory]
        //[InlineData(1, "9AA37330-3DA9-473D-9392-08723A3EC226")]
        //public async Task GetChatsHandlerTest(long accountId, Guid documentId)
        //{
        //    var handler = new GetChatsQueryHandler(_context, _options.Object, _contextAccessorMock);
        //    var result = await handler.Handle(GetChatsQuery.Create(accountId, documentId), CancellationToken.None);
        //    result.ShouldBeOfType<ChatMessengerDTO[]>();
        //}
    }
}
