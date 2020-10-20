using MediatR;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Market.EventBus.RMQ.Shared.Events.Users;
using SP.Messenger.Common.Implementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Chats.Commands.OrganizationChatInit
{
    public class OrganizationChatInitCommand : IRequest<ProcessingResult<bool>>
    {
        public IEnumerable<UserCreatedEvent> Accounts { get; }

        public IEnumerable<OrganizationCreatedEvent> Organizations { get; }

        public OrganizationChatInitCommand(IEnumerable<UserCreatedEvent> _accounts,
            IEnumerable<OrganizationCreatedEvent> _organizations) 
        {
            Accounts = _accounts;
            Organizations = _organizations;
        }

        public static OrganizationChatInitCommand Create(IEnumerable<UserCreatedEvent> _accounts,
            IEnumerable<OrganizationCreatedEvent> _organizations)
            => new OrganizationChatInitCommand(_accounts, _organizations);
    }
}
