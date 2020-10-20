using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Shouldly;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChatType;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Infrastructure.Services.Cache;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.ChatType
{
    [Collection("QueryCollection")]
    public class GetChatTypeTest
    {
        private readonly MessengerDbContext _context;
        private readonly ICacheService _cache;
        private readonly Mock<IDistributedCache> _distributedCache;

        public GetChatTypeTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _distributedCache = new Mock<IDistributedCache>();
            _cache = new CacheService(_distributedCache.Object);
        }

        [Fact]
        public async Task GetChatTypesHandlerTest()
        {
            long chatId = 48;
            var handler = new GetChatTypeByChatQueryHandler(_context, _cache);
            var result = await handler.Handle(GetChatTypeByChatQuery.Create(chatId), CancellationToken.None);
            result.ShouldBeOfType<ChatTypeDTO>();
        }
    }
}