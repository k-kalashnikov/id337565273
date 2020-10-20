using System;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using SP.Consumers.Models;
using SP.Market.Core.Extension;
using SP.Market.Core.Messages.Bulders;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Application.Messages.Command.Remove;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;
using Xunit;

namespace SP.Messenger.Application.Test.Messages
{
    [Collection("QueryCollection")]
    public class MessageCRUDCommandTest
    {
        private readonly MessengerDbContext _context;
        
        public MessageCRUDCommandTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
        }

        [Theory]
        [InlineData(73181, 1, "stec.superuser@mail.ru", "c6c1568b-dbac-4c6d-99cd-5bacf4d4f872")]
        public async Task SaveMessageTest(long chatId, long accountId, string userName, Guid documentId)
        {
            var messageClient = Create.MessageClient(chatId, documentId, string.Empty,
                new MessageClient
                {
                    Author = new Author(1,userName,"super","user",string.Empty),
                    Commands = null,
                    Content = "unitTest",
                    Date = DateTime.UtcNow,
                    File = null,
                    Readed = false,
                    ButtonCommands = null,
                    ChatId = chatId,
                    Pined = false,
                    DocumentId = documentId,
                    ModuleName = ModuleName.Logistic,
                    ChatTypeMnemonic = string.Empty,
                    MessageType = MessageTypeClient.User
                },
                null,null);//MessageTypeClient.User,
            var message = new Message
            {
                AccountId = accountId,
                ChatId = chatId,
                CreateDate = DateTime.UtcNow,
                MessageTypeId = 2,
                Content = messageClient.ToJson()
            };
            
            var handler = new SaveMessageCommandHandler(_context);
            var result = await handler.Handle(SaveMessageCommand.Create(message, userName),
                    CancellationToken.None);
            
            result.ShouldBeOfType<ProcessingResult<long>>();
        }
       
        [Theory]
        [InlineData(1, 613)]
        public async Task RemoveMessageTest(long accountId, long messageId)
        {
            var handler = new RemoveMessageCommandHandler(_context);
            var result =
                await handler.Handle(RemoveMessageCommand.Create(accountId, messageId), CancellationToken.None);
            result.ShouldBeOfType<ProcessingResult<bool>>();
        }
    }
}