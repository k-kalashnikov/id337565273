using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Chats.Commands.AddExistAccountToExistChats
{
    public class AddExistAccountToExistChatsCommand : IRequest<bool>
    {
        public long AccountId { get; }

        public IEnumerable<string> Roles { get; }


        public AddExistAccountToExistChatsCommand(long _accountId, IEnumerable<string> _roles) 
        {
            AccountId = _accountId;
            Roles = _roles;
        }

        public static AddExistAccountToExistChatsCommand Create(long _accountId, IEnumerable<string> _roles)
            => new AddExistAccountToExistChatsCommand(_accountId, _roles);
    }
}
