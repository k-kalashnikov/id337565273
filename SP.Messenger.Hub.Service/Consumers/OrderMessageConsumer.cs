using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Common.Settings;
using System;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class OrderMessageConsumer : IConsumer<MessageServerIntegrationEvent>
    {
        private readonly IMediator _mediator;
        private readonly IBusControl _bus;
        private readonly IOptions<Settings> _options;

        public OrderMessageConsumer(IMediator mediator,
            IBusControl bus, IOptions<Settings> options)
        {
            _mediator = mediator;
            _bus = bus;
            _options = options;
        }

        public Task Consume(ConsumeContext<MessageServerIntegrationEvent> context)
        {
            try
            {
                //await (Task)typeof(OrderMessageConsumer)
                //   .GetMethod(context.Message.Method)
                //   .Invoke(this, new object[]
                //   {
                //       context.Message.Messages,
                //       context.Message.UserName
                //   });
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return Task.CompletedTask;
        }

        //public async Task Save(Message message, string userName)
        //{
        //    var accounts = await _mediator.Send(new GetAllAccountsQuery());
        //    var account = accounts.FirstOrDefault(x => x.Login == message.Author.login);

        //    var chat = await _mediator.Send(new GetChatByDocumentIdQuery(Guid.Parse(message.OrderId)));
            
        //    var messageDbModel = MessageConvertor.From(message, chat.ChatId, account.AccountId);

        //    var operationResult = await _mediator.Send(new SaveMessangeCommand(messageDbModel, message.OrderId));

        //    if (operationResult.Result != 0)
        //    {
        //       // Message[] models = await _messagesService.GetMessagesByTime(message.MessageTimeId);

        //        await _bus.Publish(new MessageClientIntegrationEvent
        //        {
        //            Id = Guid.NewGuid(),
        //            CreationDate = DateTime.UtcNow,
        //            Messages = new Core.Messages.Message[] { message },
        //            Method = MethodConstEvent.SendMessagesUserAll,
        //            UserName = userName
        //        });
        //    }
        //}

        //public async Task GetMessage(Message message, string userName)
        //{
        //    //Log.Information($"GetMessage by order: {message.OrderId}");

        //    //Message[] models = await _messagesService.GetMessagesByOrder(Guid.Parse(message.OrderId));
        //    //Log.Information($"Messages : {models?.Length ?? 0}");
        //    //Log.Information($"Messages send cliet");
        //    //await _bus.Publish(new MessageClientIntegrationEvent
        //    //{
        //    //    Id = Guid.NewGuid(),
        //    //    CreationDate = DateTime.UtcNow,
        //    //    Messages = models,
        //    //    Method = MethodConstEvent.SendMessagesUser,
        //    //    UserName = userName
        //    //});

        //    //Log.Information($"Messages sent cliet");
        //}
    }
}
