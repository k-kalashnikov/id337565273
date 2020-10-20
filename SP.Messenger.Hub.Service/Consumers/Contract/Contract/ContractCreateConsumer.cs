using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Contract.Events.Event.CreateContract;
using SP.Messenger.Application.Chats.Commands.CreateContractChat;

namespace SP.Messenger.Hub.Service.Consumers.Contract.Contract
{
    public class ContractCreateConsumer : IConsumer<CreateContractEvent>
    {
        private readonly IMediator _mediator;
        
        public ContractCreateConsumer(IMediator mediator)
            =>_mediator = mediator;

        public async Task Consume(ConsumeContext<CreateContractEvent> context)
        {
            Log.Information($"{nameof(ContractCreateConsumer)} invoked");
            var request = context.Message;

            var createContractChatCommand = new CreateContractChatCommand
            (
               request.Id,
               request.Parent,
               request.ContractStatusId,
               request.ContractTypeId,
               request.CustomerOrganizationId,
               request.ContractorOrganizationId,
               request.Number,
               request.StartDate,
               request.FinishDate,
               request.SignedByCustomer,
               request.SignedByContractor
            );
            var result = await _mediator.Send(createContractChatCommand, context.CancellationToken);

            var response = new ChatInfoResponse
            {
                ChatName = $"Договор №{request.Number}",
                ChatId = result.Result,
                DocumentId = request.Id,
                ParentDocumentId = null,
                ChatTypeMnemonic = "module.contract.chat.contract"
            };

            await context.RespondAsync(CreateChatResponse.Create(response));
            Log.Information($"{nameof(ContractCreateConsumer)} complited ChatId = {response.ChatId} ChatName={response.ChatName}");
        }
    }
}
