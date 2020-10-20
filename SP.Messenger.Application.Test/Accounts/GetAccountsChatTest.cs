using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Shouldly;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Queries.GetAccountsChat;
using SP.Messenger.Application.Accounts.Queries.GetAccountsFromPlatform;
using SP.Messenger.Application.Interfaces;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Infrastructure.Services.Cache;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.Accounts
{
    [Collection("QueryCollection")]
    public class GetAccountsChatTest : TestPopstgresBase
    {
        private readonly Mock<IMediator> _mediator;
        private readonly ICacheService _cache;
        private readonly Mock<IDistributedCache> _distributedCache;

        public GetAccountsChatTest()
        {
            _mediator = new Mock<IMediator>(behavior: MockBehavior.Default);
            _distributedCache = new Mock<IDistributedCache>();
            _cache = new CacheService(_distributedCache.Object);
        }

        //[Theory]
        //[InlineData(1,1)]
        //public async Task GetAccountsHandlerTest(long chatId, long organizationId)
        //{
        //    //_mediator.Setup(m => m.Send(It.IsAny<GetAccountsFromPlatformQuery>(),
        //    //        It.IsAny<CancellationToken>()))
        //    //    .ReturnsAsync(new[]
        //    //    {
        //    //        new AccountMessengerDTO
        //    //        {
        //    //            Login = "stec.superuser@mail.ru",
        //    //            AccountId = 1,
        //    //            FirstName = "super",
        //    //            LastName = "user"
        //    //        }
        //    //    });

        //    //using var context = GetDbContext();
        //    //var handler = new GetAccountByChatHandler(context, _mediator.Object);
        //    //var result = await handler.Handle(GetAccountByChat.Create(chatId, organizationId), CancellationToken.None);
        //    //result.ShouldBeOfType<AccountMessengerDTO[]>();
        //}
    }
}