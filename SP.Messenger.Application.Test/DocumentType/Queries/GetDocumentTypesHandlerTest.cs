using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Shouldly;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Application.Chats.Queries.GetChatType;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Infrastructure.Services.Cache;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.DocumentType.Queries
{
    [Collection("QueryCollection")]
    public class ChatTypesHandlerTest
    {
        private readonly MessengerDbContext _context;
        private readonly ICacheService _cache;
        private readonly Mock<IDistributedCache> _distributedCache;

        public ChatTypesHandlerTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _distributedCache = new Mock<IDistributedCache>();
            _cache = new CacheService(_distributedCache.Object);
        }

        [Fact]
        public async Task GetChatTypesHandlerTest()
        {
            var mnemonic = "module.bidCenter.chat.common";

            var handler = new GetChatTypeMnemonicQueryHandler(_context, _cache);
            var result = await handler.Handle(new GetChatTypeMnemonicQuery(mnemonic), CancellationToken.None);
            result.ShouldBeOfType<ChatTypeDTO>();
        }
    }
}
