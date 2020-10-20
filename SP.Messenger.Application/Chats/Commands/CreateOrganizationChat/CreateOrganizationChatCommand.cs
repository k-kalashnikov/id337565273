using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Domains.Entities;
using System;
using System.Collections.Generic;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Chats.Commands.CreateOrganizationChat
{
    public class CreateOrganizationChatCommand : IRequest<ProcessingResult<long>>
    {
        public ICollection<AccountMessengerDTO> Accounts { get; }
        public string Name { get; }
        public string ChatTypeMnemonic { get;  }
        public ChatData ChatData { get; }

        public CreateOrganizationChatCommand(string _name, ICollection<AccountMessengerDTO> _accounts, string _chatTypeMnemonic, ChatData _chatData) 
        {
            Accounts = _accounts;
            Name = _name;
            ChatTypeMnemonic = _chatTypeMnemonic;
            ChatData = _chatData;
        }

        public static CreateOrganizationChatCommand Create(string _name, 
            ICollection<AccountMessengerDTO> _accounts,
            string _chatTypeMnemonic,
            string _objectId,
            string _objectTypeId) 
        {

            var chatData = new ChatData() {
                DocumentId = _objectId,
                DocumentStatusMnemonic = _objectTypeId,
                DocumentTypeId = 7
            };

            return new CreateOrganizationChatCommand(_name, _accounts, _chatTypeMnemonic, chatData);
        }
    }
}
