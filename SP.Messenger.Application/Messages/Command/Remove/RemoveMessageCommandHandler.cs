using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Messages.Validators;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Messages.Command.Remove
{
    public class RemoveMessageCommandHandler : IRequestHandler<RemoveMessageCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;
        
        public RemoveMessageCommandHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<ProcessingResult<bool>> Handle(RemoveMessageCommand request, CancellationToken cancellationToken)
        {
            var model = await _context.Messages.FirstOrDefaultAsync(
                x=>x.MessageId.Equals(request.MessageId), cancellationToken);

            var validatorResponse = MessageValidator.RemoveRule.RemoveMessageCheck(model, request.AccountId);
            if (validatorResponse.Result != true)
                return validatorResponse;
            
            _context.Messages.Remove(model);
            await _context.SaveChangesAsync(cancellationToken);
            return new ProcessingResult<bool>(true);
        }
    }
}