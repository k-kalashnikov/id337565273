using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using SP.Messenger.Application.Files.Models;
using SP.Messenger.Application.Files.Queries.GetFilesByDocument;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Settings;
using Xunit;

namespace SP.Messenger.Application.Test.Files.Queries.GetFilesByDocument
{
    public class GetFilesByDocumentTest : TestPopstgresBase
    {
        private Mock<IOptions<Settings>> _options;

        public GetFilesByDocumentTest()
        {
            _options = new Mock<IOptions<Settings>>();
        }
        [Theory]
        [InlineData("dee79776-3173-4534-8be8-4c2e30aef50d")]
        public async Task GetFilesByDocumentHandlerTest(Guid documentId)
        {
            var settings = new Settings
            {
                RMQClient = new RMQClient(),
                Logging = new Logging(),
                Templates = new Templates(),
                AllowedHosts = "*",
                ConnectionString = new ConnectionString(),
                FileServer = new FileServer
                {
                    Storage = "http://localhost:5005/api/v1/messenger"
                }
            };
            _options.Setup(m => m.Value)
                .Returns(settings);
            
            using (var context = GetDbContext(initialize: false))
            {
                var handler = new GetFilesByDocumentQueryHandler(context, _options.Object);
                var result = await handler.Handle(new GetFilesByDocumentQuery(documentId), CancellationToken.None);
                result.ShouldBeOfType<FileShortDto[]>();   
            }
        }
    }
}