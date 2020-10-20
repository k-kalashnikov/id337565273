using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Commands;
using SP.Messenger.Application.Chats.Commands.AddNewAccountsToExistsChats;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Infrastructure.Services.Accounts;
using SP.Messenger.Persistence;
using UserEvents = SP.Market.EventBus.RMQ.Shared.Events.Users;

namespace SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UserCreated
{
  public class UserCreatedConsumer : IConsumer<UserEvents.UserCreatedEvent>
  {
    private readonly IMediator _mediator;
    private readonly MessengerDbContext _context;
    private readonly IAccountsOrganizationService<GetAccountsByOrganizationRequest, GetAccountsByOrganizationResponse[]> _accountsService;

    public UserCreatedConsumer(IMediator mediator)
    {
      _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Consume(ConsumeContext<UserEvents.UserCreatedEvent> context)
    {
      var command = AddNewAccountsToExistsChatsCommand.Create(
          context.Message.AccountId,
          context.Message.Login.ToLower(),
          context.Message.FirstName,
          context.Message.LastName,
          context.Message.MiddleName,
          context.Message.Organization,
          context.Message.Roles.ToArray(),
          context.Message.OrganizationGuid);

      await _mediator.Send(command, context.CancellationToken);
    }
  }
}
