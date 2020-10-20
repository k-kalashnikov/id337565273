using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using MODULE = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.ReportCreateChatProject
{
    public class CreateChatReportProjectConsumer : IConsumer<CreateChatReportProjectRequest>
    {
        private readonly IMediator _mediator;
        public CreateChatReportProjectConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<CreateChatReportProjectRequest> context)
        {
            var accounts = Array.Empty<AccountMessengerDTO>();

            var command = CreateChatCommand.Create(context?.Message?.ProjectName,
                context?.Message?.ChatTypeMnemonic, 
                true, 
                context?.Message?.ProjectId ?? Guid.Empty,
                context?.Message?.DocumentTypeId ?? 0,
                context?.Message?.DocumentStatusMnemonic,
                Enum.Parse<MODULE>(context?.Message?.Module), accounts);

            var resultCommand = await _mediator.Send(command, CancellationToken.None);

            if (context != null)
            {
                var response = CreateChatReportProjectResponse.Create(resultCommand.Result);
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