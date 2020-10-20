using Microsoft.AspNetCore.Http;
using SP.Messenger.Persistence;
using System;
using Xunit;

namespace SP.Messenger.Application.Test.Infrastructure
{
    public class QueryTestFixture : IDisposable
    {
        public MessengerDbContext Context { get; private set; }
        public IHttpContextAccessor ContextAccessor { get; private set; }
        public QueryTestFixture()
        {
            Context = MessengerContextFactory.Create();
            ContextAccessor = MockFactory.CreateHttpContextAccessorMock();
        }

        public void Dispose()
        {
            MessengerContextFactory.Destroy(Context);
        }
    }

    [CollectionDefinition("QueryCollection")]
    public class QueryCollection : ICollectionFixture<QueryTestFixture> { }
}
