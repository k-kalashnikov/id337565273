using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Shouldly;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Commands;
using SP.Messenger.Application.Chats.Commands.AddNewAccountsToExistsChats;
using SP.Messenger.Application.Test.Infrastructure;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Persistence;
using Xunit;

namespace SP.Messenger.Application.Test.Chat.AddNewAccountsToExistsChats
{
    [Collection("QueryCollection")]
    public class AddNewAccountsToExistsChatsTest
    {
        private readonly MessengerDbContext _context;
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<IAccountsOrganizationService<GetAccountsByOrganizationRequest,
            GetAccountsByOrganizationResponse[]>> _accountsService;
        
        public AddNewAccountsToExistsChatsTest(QueryTestFixture fixture)
        {
            _context = fixture.Context;
            _mediator = new Mock<IMediator>();
            _accountsService = new Mock<IAccountsOrganizationService<GetAccountsByOrganizationRequest,
                GetAccountsByOrganizationResponse[]>>();
        }
        
        [Theory]
        [InlineData(1, 221)]
        public async Task AddNewAccountsToExistsChats(long accountId, long? organizationId)
        {
            var login = "test@test.ru";
            var firstName = "test";
            var lastName = "testov";
            var middleName = "testovich";
            var roles = new[] {"admin"};
            
            var command = AddNewAccountsToExistsChatsCommand.Create(accountId,
                login,
                firstName,
                lastName,
                middleName,
                organizationId,
                roles,
                Guid.NewGuid());

            _mediator.Setup(m => m.Send(It.IsAny<SaveAccountCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(accountId);

            var response = new GetAccountsByOrganizationResponse
            {
                AccountId = accountId,
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName
            };

            _accountsService.Setup(m
                => m.GetAccountByOrganizationIds(It.IsAny<GetAccountsByOrganizationRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new []{response});
            
            
            
             //var handler = new AddNewAccountsToExistsChatsCommandHandler(_mediator.Object, _context, _accountsService.Object, null);
             //var result = await handler.Handle(command, CancellationToken.None);
             //result.ShouldBeOfType<bool>();
             //Assert.True(result);
        }
    }
}