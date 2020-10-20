using MassTransit.Testing;
using MediatR;
using Moq;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.BidCreateInviteChats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SP.Messenger.Integration.Test
{
    public class Should_test_the_consumer
    {
        Mock<IMediator> _mediator;
        public Should_test_the_consumer()
        {
            _mediator = new Mock<IMediator>();
        }
        [Fact]
        public async Task Should_test_the_BidCreateInviteChatsConsumer()
        {
            //_mediator.Setup(x => x.Publish(It.IsAny<GetAccountsByOrganizationIdsQuery>(), It.IsAny<CancellationToken>()))
            //   .Verifiable("");

            //var harness = new InMemoryTestHarness();
            //var consumerHarness = harness.Consumer(
            //    ()=>new BidCreateInviteChatsConsumer(_mediator.Object),
            //    nameof(BidCreateInviteChatsRequest));

            //await harness.Start();
            //try
            //{
            //    var message = new BidCreateInviteChatsRequest(
            //        new ChatPerformer[] 
            //        {
            //            new ChatPerformer 
            //            {
            //                AuthorId = 1,
            //                ChatTypeMnemonic = "module.market.pusrchase.chat.private",
            //                DocumentId = Guid.Empty,
            //                DocumentStatusMnemonic = "",
            //                DocumentTypeId = 1,
            //                Module = "Market",
            //                Name = "dscvsdv",
            //                ParentDocumentId = Guid.Empty,
            //                PerformerId = 123
            //            } 
            //        });
            //    await harness.InputQueueSendEndpoint.Send(message);

            //    // did the endpoint consume the message
            //    //Assert.IsTrue(harness.Consumed.Select<BidCreateInviteChatsRequest>().Any());

            //    // did the actual consumer consume the message
            //    //Assert.IsTrue(consumerHarness.Consumed.Select<BidCreateInviteChatsRequest>().Any());
            //}
            //finally
            //{
            //    await harness.Stop();
            //}
        }
    }
}
