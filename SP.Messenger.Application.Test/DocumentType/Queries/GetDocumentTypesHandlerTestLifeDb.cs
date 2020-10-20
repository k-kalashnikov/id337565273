using Shouldly;
using SP.Messenger.Application.DocumentType.Queries;
using SP.Messenger.Application.Test.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Application.Test.DocumentType.Queries
{
    public class GetDocumentTypesHandlerTestLifeDb : TestPopstgresBase
    {
        [Fact]
        public async Task GetDocumentTypesTestLifeDb()
        {
            using (var context = GetDbContext())
            {
                var handler = new GetDocumentTypesHandler(context);
                var result = await handler.Handle(new GetDocumentTypes(), CancellationToken.None);
                result.ShouldBeOfType<DocumentTypeDTO[]>();
                Assert.NotEmpty(result);
            }
        }
    }
}
