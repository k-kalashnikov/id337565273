using MediatR;
using SP.Consumers.Models;
using System;
using System.Collections.Generic;

namespace SP.Messenger.Application.Chats.Commands.AddPerformersToChatsInvite
{
    public class AddPerformersToChatsInviteCommand : IRequest<bool>
    {
        public IEnumerable<AccountMessengerDTO> Accounts { get; }
        public Guid DocumentInviteId { get; }

        public AddPerformersToChatsInviteCommand(IEnumerable<AccountMessengerDTO> accounts, Guid documentInviteId)
        {
            Accounts = accounts;
            DocumentInviteId = documentInviteId;
        }
    }
}
