using Microsoft.AspNetCore.Http;
using Moq;

namespace SP.Messenger.Application.Test.Infrastructure
{
    public class MockFactory
    {
        public static IHttpContextAccessor CreateHttpContextAccessorMock()
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var fakeTenantId = "abcd";
            context.Request.Headers["Tenant-ID"] = fakeTenantId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            //Mock HeaderConfiguration
            //var mockHeaderConfiguration = new Mock<IHeaderConfiguration>();
            //mockHeaderConfiguration
            //    .Setup(_ => _.GetTenantId(It.IsAny<IHttpContextAccessor>()))
            //    .Returns(fakeTenantId);

            return mockHttpContextAccessor.Object;
        }
    }
}
