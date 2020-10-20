using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccounts;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccountsObject
{
    public class UpdateAccountsObjectCommandHandler : IRequestHandler<UpdateAccountsObjectCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly MessengerDbContext _context;
        public UpdateAccountsObjectCommandHandler(IMediator mediator, MessengerDbContext context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<bool> Handle(UpdateAccountsObjectCommand request, CancellationToken cancellationToken)
        {
            var chatDb = await _context.ChatView
                .FirstOrDefaultAsync(x => x.DocumentId.Equals(request.ProjectId.ToString()),
                    cancellationToken);
            
            var command = UpdateAccountsCommand.Create(request.ProjectId, chatDb.ChatId);
            var result = await _mediator.Send(command, cancellationToken);
            return result;
        }
    }
}