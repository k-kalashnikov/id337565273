using System;
using MediatR;

namespace SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccounts
{
    public class UpdateAccountsCommand : IRequest<bool>
    {
        public UpdateAccountsCommand(Guid projectId, long chatId)
        {
            ProjectId = projectId;
            ChatId = chatId;
        }
        public Guid ProjectId { get; }
        public long ChatId { get; }
        
        public static UpdateAccountsCommand Create(Guid projectId, long chatId)
            => new UpdateAccountsCommand(projectId, chatId);
    }
}