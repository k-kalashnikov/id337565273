using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Market.Core.Extension;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Messages.Command.CloseBotMessage
{
    public class CloseBotMessageCommandHandler : IRequestHandler<CloseBotMessageCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;

        public CloseBotMessageCommandHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<ProcessingResult<bool>> Handle(CloseBotMessageCommand request, CancellationToken cancellationToken)
        {
            var message = await _context.Messages
                .SingleOrDefaultAsync(x => x.MessageId.Equals(request.MessageId), cancellationToken);
            
            if (message is null)
            {
                return new ProcessingResult<bool>(false, new []
                {
                    new SimpleResponseError($"Message not found by MessageId: {request.MessageId}")
                });
            }
            
            var metadataMessage = message.Content.FromJson<ContentMessage>();
            metadataMessage.CommandClient.Open = OpenCommand.Closed;
            metadataMessage.CommandClient.Value = request.Value ?? 0;
            metadataMessage.Content = request.Content;
            message.Content = metadataMessage.ToJson();
            
            _context.Messages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
            
            return new ProcessingResult<bool>(true);
        }
    }
}