using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Chats.Commands.CreateChat.PostProcess
{
    public class CreateChatPostProcessor : IRequestPostProcessor<CreateChatCommand, long>
    {
        private readonly MessengerDbContext _context;
       
        
        public CreateChatPostProcessor(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public Task Process(CreateChatCommand request, long response, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}