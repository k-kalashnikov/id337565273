using System;
using MassTransit;
using MassTransit.RabbitMqTransport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SP.Consumers.Models;
using SP.Contract.Events.Event.CreateContract;
using SP.Contract.Events.Request.GetOrganizationContracts;
using SP.Logistic.Events.Events.Exchange;
using SP.Logistic.Events.Events.TransportOrder;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Market.EventBus.RMQ.Shared.Events.Messenger;
using SP.Market.EventBus.RMQ.Shared.Events.Need;
using SP.Market.EventBus.RMQ.Shared.Events.Purchase;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Purchases.GetPurchases;
using SP.Messenger.Events.Requests;
using SP.Messenger.Hub.Service.Consumers;
using SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UpdateChatAccounts;
using SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UserCreated;
using SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UserUpdated;
using SP.Messenger.Hub.Service.Consumers.Accounts.Queries.GetAccounts;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.BidCreateChat;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.BidCreateInviteChats;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.MarketCreateChatPurchase;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.MarketCreateChatVote;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.ReportCreateChatProject;
using SP.Messenger.Hub.Service.Consumers.Chats.Commands.ReportCreateChatQuestion;
using SP.Messenger.Hub.Service.Consumers.Chats.Queries.GetChatInfo;
using SP.Messenger.Hub.Service.Consumers.ChatTypes.Queries.GetChatTypes;
using SP.Messenger.Hub.Service.Consumers.CompletedProtocol;
using SP.Messenger.Hub.Service.Consumers.Contract.Contract;
using SP.Messenger.Hub.Service.Consumers.GetLastMessages;
using SP.Messenger.Hub.Service.Consumers.Messages.Commands.CloseBotMessage;
using SP.Messenger.Hub.Service.Consumers.Messages.Commands.NeedRemove;
using SP.Messenger.Hub.Service.Consumers.Messages.Commands.Pin;
using SP.Messenger.Hub.Service.Consumers.Messages.Commands.PurchaseUpdateStatusToOffers;
using SP.Messenger.Hub.Service.Consumers.PanelButtons;
using SP.Messenger.Hub.Service.Observers;
using SP.Messenger.Hub.Service.Services;
using SP.Protocol.Events.Common.Event.Protocol.CompletedProtocolReport;
using SP.Purchase.Events.Purchase;
using UserEvents = SP.Market.EventBus.RMQ.Shared.Events.Users;


namespace SP.Messenger.Hub.Service.Extensions
{
	public static class MassTransitStartupExtensions
	{
		public static IServiceCollection AddMassTransitService(this IServiceCollection services, IConfiguration config)
		{
			var timeout = TimeSpan.FromSeconds(30);

			services.AddMassTransit(x =>
			{
				x.AddConsumer<OrderMessageConsumer>();
				x.AddConsumer<MessengerCommonConsumer>(); //удалить
				x.AddConsumer<MessengerFromBotConsumer>();
				x.AddConsumer<BidCreateChatConsumer>();
				x.AddConsumer<GetAccountsChatConsumer>();
				x.AddConsumer<GetLastMessagesConsumer>();
				x.AddConsumer<GetChatTypesConsumer>();
				x.AddConsumer<GetChatInfoConsumer>();
				x.AddConsumer<CreateChatReportProjectConsumer>();
				x.AddConsumer<CreateChatReportQuestionConsumer>();
				x.AddConsumer<PinMassageConsumer>();
				x.AddConsumer<UpdateChatAccountsConsumer>();
				x.AddConsumer<CloseBotMessageConsumer>();
				x.AddConsumer<BidCreateInviteChatsConsumer>();
				x.AddConsumer<UserCreatedConsumer>();
				x.AddConsumer<MessengerConsumer>();
				x.AddConsumer<PanelButtonCommandsConsumer>();
				x.AddConsumer<MarketCreateChatVoteConsumer>();
				x.AddConsumer<MarketCreateChatPurchaseConsumer>();
				x.AddConsumer<OrganizationCreateChatConsumer>();
				x.AddConsumer<CompletedProtocolReportConsumer>();
				x.AddConsumer<OrganizationChatsInit>();
				x.AddConsumer<UserUpdatedConsumer>();
				x.AddConsumer<PurchaseUpdateStatusToOffersConsumer>();
				x.AddConsumer<PurchaseCreateConsumer>();
				x.AddConsumer<NeedRemoveConsumer>();

				#region LogisticConsumers
				x.AddConsumer<LogisticOrderCreateConsumer>();
				x.AddConsumer<LogisticSuborderCommissionSendingConsumer>();
				x.AddConsumer<LogisticSuborderCreateConsumer>();
				x.AddConsumer<LogisticSuborderTradingStartConsumer>();
				#endregion

				#region PurchaseConsumers
				x.AddConsumer<PurchaseRefusedConsumer>();
				#endregion


				#region MDMConsumers
				x.AddConsumer<ContractCreateConsumer>();
				#endregion

				x.AddRequestClient<RequestAccountsByOrganization>(timeout);
				x.AddRequestClient<GetCurrentBidStatusRequest>(timeout);
				x.AddRequestClient<GetContragentsByProjectRequest>(timeout);
				x.AddRequestClient<GetAccountIdentityRequest>(timeout);
				x.AddRequestClient<GetChatInfoRequest>(timeout);
				x.AddRequestClient<GetBidByTransportSubOrderIdRequest>(timeout);
				x.AddRequestClient<GetAccountsByIdsRequest>(timeout);
				x.AddRequestClient<GetAccountsByRolesRequest>(timeout);
				x.AddRequestClient<GetUsersByRoleRequest>(timeout);
				x.AddRequestClient<GetAccountsByOrganizationIdRequest>(timeout);
				x.AddRequestClient<GetAccountsByOrganizationRequest>(timeout);
				x.AddRequestClient<GetAccountsByOrganizationIdsRequest>(timeout);

				#region MDMClients
				x.AddRequestClient<GetOrganizationsRequest>(timeout);
				#endregion

				#region AccountClients
				x.AddRequestClient<GetRolesByAccountIdRequest>(timeout);
				#endregion

				#region ContractClients
				x.AddRequestClient<GetOrganizationContractsRequest>(timeout);
				#endregion

				#region ProtocolClients
				x.AddRequestClient<CreateProtocolCommissionRequest>(timeout);
				#endregion

				#region MarketClients
				x.AddRequestClient<GetPurchasesRequest>(timeout);
				#endregion


				x.AddBus(provider =>
				  {
					  var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
				{
							  cfg.Host(new Uri($"rabbitmq://{config["RMQClient:Host"]}/"), hostConfig =>
						{
									  hostConfig.Username(config["RMQClient:UserName"]);
									  hostConfig.Password(config["RMQClient:Password"]);
								  });

							  AddReceiveEndpoint<OrderMessageConsumer>(nameof(MessageServerIntegrationEvent), cfg, provider);
							  AddReceiveEndpoint<MessengerCommonConsumer>(nameof(MessengerServerEvent), cfg, provider);
							  AddReceiveEndpoint<MessengerFromBotConsumer>(nameof(MessengerBotClientEvent), cfg, provider);
							  AddReceiveEndpoint<BidCreateChatConsumer>(nameof(MessengerClientFromWorkflowtCreateChatEvent), cfg, provider);
							  AddReceiveEndpoint<GetAccountsChatConsumer>(nameof(RequestChat), cfg, provider);
							  AddReceiveEndpoint<GetLastMessagesConsumer>(nameof(GetLastMessagesRequest), cfg, provider);
							  AddReceiveEndpoint<GetChatTypesConsumer>(nameof(GetChatTypeRequest), cfg, provider);
							  AddReceiveEndpoint<GetChatInfoConsumer>(nameof(GetChatInfoRequest), cfg, provider);
							  AddReceiveEndpoint<CreateChatReportProjectConsumer>(nameof(CreateChatReportProjectRequest), cfg, provider);
							  AddReceiveEndpoint<CreateChatReportQuestionConsumer>(nameof(CreateChatReportQuestionRequest), cfg, provider);
							  AddReceiveEndpoint<PinMassageConsumer>(nameof(MessengerPinMessageEvent), cfg, provider);
							  AddReceiveEndpoint<UpdateChatAccountsConsumer>(nameof(UpdateChatAccountsRequest), cfg, provider);
							  AddReceiveEndpoint<CloseBotMessageConsumer>(nameof(CloseBotMessageRequest), cfg, provider);
							  AddReceiveEndpoint<BidCreateInviteChatsConsumer>(nameof(BidCreateInviteChatsRequest), cfg, provider);
							  AddReceiveEndpoint<UserCreatedConsumer>(nameof(UserEvents.UserCreatedEvent), cfg, provider);

							  AddReceiveEndpoint<MessengerConsumer>(nameof(MessengerClientEvent), cfg, provider);
							  AddReceiveEndpoint<PanelButtonCommandsConsumer>(nameof(PanelButtonCommandsRequest), cfg, provider);
							  AddReceiveEndpoint<MarketCreateChatVoteConsumer>(nameof(MarketCreateChatVoteRequest), cfg, provider);
							  AddReceiveEndpoint<OrganizationCreateChatConsumer>(nameof(OrganizationCreatedEvent), cfg, provider);
							  AddReceiveEndpoint<MarketCreateChatPurchaseConsumer>(nameof(CreateMarketPurchaseChatRequest), cfg, provider);
							  AddReceiveEndpoint<CompletedProtocolReportConsumer>(nameof(CompletedProtocolReportEvent), cfg, provider);
							  AddReceiveEndpoint<OrganizationChatsInit>(nameof(OrgnizationChatInitEvent), cfg, provider);
							  AddReceiveEndpoint<UserUpdatedConsumer>(nameof(UserEvents.UserUpdatedEvent), cfg, provider);
							  AddReceiveEndpoint<PurchaseUpdateStatusToOffersConsumer>(nameof(PurchaseUpdateStatusToOffersCollectEvent), cfg, provider);
							  AddReceiveEndpoint<PurchaseCreateConsumer>(nameof(CreatedPurchaseEvent), cfg, provider);
							  AddReceiveEndpoint<NeedRemoveConsumer>(nameof(RemoveNeedFromPurchaseEvent), cfg, provider);

						#region LogisticEndpoints
						AddReceiveEndpoint<LogisticOrderCreateConsumer>(nameof(TransportOrderCreatedEvent), cfg, provider);
							  AddReceiveEndpoint<LogisticSuborderCommissionSendingConsumer>(nameof(CommissionSendingEvent), cfg, provider);
							  AddReceiveEndpoint<LogisticSuborderCreateConsumer>(nameof(TransportSuborderCreatedEvent), cfg, provider);
							  AddReceiveEndpoint<LogisticSuborderTradingStartConsumer>(nameof(TradingStartEvent), cfg, provider);
						#endregion

						#region PurchaseEndpoints
						AddReceiveEndpoint<PurchaseRefusedConsumer>(nameof(PurchaseRefusedEvent), cfg, provider);
						#endregion

						#region MDMEndpoints
						AddReceiveEndpoint<ContractCreateConsumer>(nameof(CreateContractEvent), cfg, provider);
						#endregion

					});

					  var serviceProvider = services.BuildServiceProvider();
					  using var scope = serviceProvider.CreateScope();
					  var servicesProvider = scope.ServiceProvider;
					  var receiveObserver = servicesProvider.GetRequiredService<IReceiveObserver>();
					  var publishObserver = servicesProvider.GetRequiredService<IPublishObserver>();

					  bus.ConnectReceiveObserver(receiveObserver);
					  bus.ConnectPublishObserver(publishObserver);

					  return bus;
				  });
			});

			services.AddHostedService<BusService>();
			services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());

			services.AddTransient<IReceiveObserver, ReceiveObserver>();
			services.AddTransient<IPublishObserver, PublishObserver>();

			return services;
		}

		private static void AddReceiveEndpoint<T>(string name,
				IRabbitMqBusFactoryConfigurator configurator, IRegistration provider)
			where T : class, IConsumer
		{
			configurator.ReceiveEndpoint(name, configure =>
			{
				configure.ConfigureConsumer<T>(provider);
			});
		}
	}
}
