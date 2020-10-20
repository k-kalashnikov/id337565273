using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using MODULE = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.ReportCreateChatQuestion
{
    public class CreateChatReportQuestionConsumer : IConsumer<CreateChatReportQuestionRequest>
    {
        private readonly IMediator _mediator;
        
        public CreateChatReportQuestionConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task Consume(ConsumeContext<CreateChatReportQuestionRequest> context)
        {
            var accounts = Array.Empty<AccountMessengerDTO>();

            var command = CreateChatCommand.Create(context?.Message?.QuestionName,
                context?.Message?.ChatTypeMnemonic, 
                true, 
                context?.Message?.QuestionId ?? Guid.Empty,
                context?.Message?.DocumentTypeId ?? 0,
                context?.Message?.DocumentStatusMnemonic,
                Enum.Parse<MODULE>(context?.Message?.Module), 
                accounts, 
                context?.Message?.ProjectId ?? Guid.Empty);

            var resultCommand = await _mediator.Send(command, CancellationToken.None);

            if (context != null)
            {
                var response = CreateChatReportQuestionResponse.Create(resultCommand.Result);
                await context.RespondAsync(response);
            }
            else
            {
                Log.Error("CreateChatConsumer Exception, context = null");
                throw new InvalidOperationException(nameof(context));
            }
        }
    }
}