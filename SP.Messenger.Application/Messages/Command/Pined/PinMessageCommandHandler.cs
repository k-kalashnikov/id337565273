using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Messages.Validators;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Messages.Command.Pined
{
    public class PinMessageCommandHandler : IRequestHandler<PinMessageCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;
        
        public PinMessageCommandHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<ProcessingResult<bool>> Handle(PinMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                    .SingleOrDefaultAsync(x => x.MessageId == request.MessageId, cancellationToken);

            var validatorResponse = MessageValidator.Entity.CheckNull(message);
            if (validatorResponse.Result != true)
                return validatorResponse;
            
            var messagesPined = await _context.Messages
                    .Where(x =>x.ChatId == message.ChatId && x.Pined)
                    .ToArrayAsync(cancellationToken);
            
            if (messagesPined.Any())
            {
                foreach (var item in messagesPined)
                    item.Pined = false;
                _context.Messages.UpdateRange(messagesPined);
            }

            message.Pined = true;
            _context.Messages.Update(message);
            
            await _context.SaveChangesAsync(cancellationToken);
            return new ProcessingResult<bool>(true);
        }
    }
}