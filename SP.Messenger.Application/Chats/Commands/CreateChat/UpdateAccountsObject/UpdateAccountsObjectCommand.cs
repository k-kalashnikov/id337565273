using System;
using MediatR;

namespace SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccountsObject
{
    public class UpdateAccountsObjectCommand : IRequest<bool>
    {
        public Guid ProjectId { get; set; }
        public static UpdateAccountsObjectCommand Create(Guid projectId)
        => new UpdateAccountsObjectCommand
        {
            ProjectId = projectId
        };
    }
}