using MediatR;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Common.Extensions;

namespace SP.Messenger.Application.Messages.Command
{
    public class SaveMessageCommandHandler : IRequestHandler<SaveMessageCommand, ProcessingResult<long>>
    {
        private readonly MessengerDbContext _context;

        public SaveMessageCommandHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ProcessingResult<long>> Handle(SaveMessageCommand request, CancellationToken cancellationToken)
        {
            Log.Information($"SaveMessageCommandHandler command: {request.Message.ToJson()}");
            var model = await _context.Messages.AddAsync(request.Message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new ProcessingResult<long>(model.Entity.MessageId);
        }
    }
}
