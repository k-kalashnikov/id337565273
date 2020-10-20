using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Market.Core.Extension;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Exceptions;
using SP.Messenger.Application.Messages.Command.SaveSystemMessage;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Client.Events;
using SP.Messenger.Client.Shared;
using VotingContractor = SP.Messenger.Client.Shared.VotingContractor;

namespace SP.Messenger.Application.Voting.Commands.SetVote
{
    public class SetVoteCommandHandler : IRequestHandler<SetVoteCommand, ProcessingResult<bool>>
    {
        private readonly MessengerDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBusControl _bus;
        private readonly IMediator _mediator;
        
        public SetVoteCommandHandler(MessengerDbContext context,
            ICurrentUserService currentUserService,
            IBusControl bus,
            IMediator mediator)
        {
            _context = context;
            _currentUserService = currentUserService;
            _bus = bus;
            _mediator = mediator;
        }

        public async Task<ProcessingResult<bool>> Handle(SetVoteCommand request, CancellationToken token)
        {
            var currentUser = _currentUserService.GetCurrentUser();
            
            var voting = await _context.Votings
                .Include(x => x.ResponseVariants)
                .Include(x => x.VotedCollection)
                .SingleOrDefaultAsync(x => x.Id == request.VoteId, token);

            if (voting is null)
                throw new NotFoundException(nameof(Voting), request.VoteId);

            if (voting.IsClosed)
                return new ProcessingResult<bool>(false, new[] { new SimpleResponseError("Голосование закрыто") });

            var chat = await GetChatById(voting.Id, token);

            voting.Like(currentUser.Id, request.ResponseVariant, request.Like, request.Comment);

            _context.Votings.Update(voting);
            await _context.SaveChangesAsync(token);

            await CheckState(chat, voting, currentUser, token);

            return new ProcessingResult<bool>(true);
        }

        private async Task CheckState(ChatView chat, Domains.Entities.Voting voting, ICurrentUser currentUser, CancellationToken token)
        {
            if (voting.IsClosed)
            {
                await PushEventFinishVoting(chat, voting, currentUser, token);
            }
            else
            {
                await ChooseWinner(chat, voting, currentUser, token);
            }
        }

        private async Task PushEventFinishVoting(ChatView chat, Domains.Entities.Voting voting, ICurrentUser currentUser, CancellationToken token)
        {
            Log.Information($"{nameof(PushEventFinishVoting)}");
            
            var likes = new List<SP.Messenger.Client.Shared.Likes>();
            var contractorIds = new List<long>();
            
            foreach (var variant in voting.ResponseVariants)
            {
                var orgContent = JsonConvert.DeserializeObject<OrganizationContent>(variant.OrganizationsContent);
                
                contractorIds.AddRange(orgContent.Organizations.Select(x=>x.ContractorId).ToList());
                var likeAccounts = voting.VotedCollection
                    .Where(x => x.ResponseVariantId == variant.Id)
                    .Select(x => new LikeAccount 
                    {
                        AccountId = x.AccountId,
                        IsLike = x.IsLike 
                    });
               
                var count = likeAccounts.Count();
                var voted = voting.VotedCollection.FirstOrDefault(x => x.ResponseVariantId == variant.Id);
                likes.Add
                (
                    new Likes
                    (
                        variantName: variant.Name,
                        decisionId: variant.DecisionId,
                        contractors: orgContent?.Organizations.Select(x => new VotingContractor
                        {
                            ContractorId = x.ContractorId,
                            ContractorName = x.ContractorName,
                            PriceOffer = x.PriceOffer,
                            DeviationBestPrice = x.DeviationBestPrice,
                            Term = x.Term,
                            TermDeviation = x.TermDeviation,
                            DefermentPayment = x.DefermentPayment,
                            DefermentDeviation = x.DefermentDeviation
                        }),
                        like: count,
                        accountIds: likeAccounts,
                        isLike: voted?.IsLike,
                        comment: voted?.Comment
                    )
                );
            }
            
            var message = FinishedVotingByDislikeMessengerEvent.Create
            (
                Guid.Parse(chat.DocumentId),
                contractorIds.ToArray(),
                likes
            );
            await _bus.Publish(message, token);
        }

        private async Task ChooseWinner(ChatView chat, Domains.Entities.Voting voting, ICurrentUser currentUser, CancellationToken token)
        {
            var listWinners = new Dictionary<Guid, int>(voting.VotedCollection.Count);
            if (voting.VotedCollection.Count(x => x.ResponseVariantId == null) == 0)
            {
                var likes = new List<Likes>();
                foreach (var variant in voting.ResponseVariants)
                {
                    var likeAccounts = voting.VotedCollection
                        .Where(x => x.ResponseVariantId == variant.Id)
                        .Select(x => new LikeAccount
                        {
                            AccountId = x.AccountId,
                            IsLike = x.IsLike
                        });
                   
                    var count = likeAccounts.Count();

                    var orgContent = JsonConvert.DeserializeObject<OrganizationContent>(variant.OrganizationsContent);
                    likes.Add
                        (
                            new Likes
                            (
                                variantName: variant.Name, 
                                decisionId: variant.DecisionId,
                                contractors: orgContent?.Organizations.Select(x=> new VotingContractor 
                                {
                                    ContractorId = x.ContractorId,
                                    ContractorName = x.ContractorName,
                                    PriceOffer = x.PriceOffer,
                                    DeviationBestPrice = x.DeviationBestPrice,
                                    Term = x.Term,
                                    TermDeviation = x.TermDeviation,
                                    DefermentPayment = x.DefermentPayment,
                                    DefermentDeviation = x.DefermentDeviation
                                }),
                                like: count,
                                accountIds: likeAccounts,
                                isLike: true,
                                comment: string.Empty
                            )
                        );

                    if (count > 0)
                        listWinners.Add(variant.Id, count);
                }
                var winner = listWinners.Max(x => x.Value);
                var key = listWinners.FirstOrDefault(x => x.Value == winner);
                
                await PushEvents(chat, voting, currentUser, likes, key, token);
            }
        }

        private async Task PushEvents(ChatView chat, Domains.Entities.Voting voting, ICurrentUser currentUser, IEnumerable<Likes> likes, KeyValuePair<Guid, int> key, CancellationToken token)
        {
            Log.Information(nameof(PushEvents));
            await PushSystemMessage(chat, currentUser, voting, key, token);
            await PushToBot(chat, currentUser, key, token);
            await PushToMarket(chat, voting, likes, key, token);
        }

        private async Task PushSystemMessage(ChatView chat, ICurrentUser currentUser, Domains.Entities.Voting voting, KeyValuePair<Guid, int> key, CancellationToken token)
        {
            // chat.ParentDocumentId  = PurchaseId
            const string mnemonicTypeChat = "module.market.pusrchase.chat.private";
            const string content = "Закупка завершена, в ближайшее время Вам будет направлен проект договора.";
            var variantWinner = voting.ResponseVariants.FirstOrDefault(x => x.Id == key.Key);
            if(variantWinner is null)
                return;
            
            var organizationWinnerList = JsonConvert.DeserializeObject<OrganizationContent>(variantWinner.OrganizationsContent);

            var chatPurchase = await _context.ChatView
                    .FirstOrDefaultAsync(x => x.ParentDocumentId == chat.ParentDocumentId, token);

            foreach (var item in organizationWinnerList.Organizations)
            {
                var chatsInvite = await _context.ChatView
                    .FirstOrDefaultAsync
                    (
                        x => x.ParentDocumentId == chatPurchase.DocumentId 
                        && x.Mnemonic == "module.market.pusrchase.chat.private"
                        && x.ContractorId == item.ContractorId,
                        token
                    );

                if (chatsInvite is null) 
                    continue;

                var command = SaveSystemMessageCommand.Create
                (
                    accountId: currentUser.Id,
                    documentId: Guid.Parse(chatsInvite.DocumentId),
                    message: content,
                    mnemonic: mnemonicTypeChat
                );
                await _mediator.Send(command, token);
            }            
        }
        private async Task PushToMarket(ChatView chat, Domains.Entities.Voting voting, IEnumerable<Likes> likes, KeyValuePair<Guid, int> key, CancellationToken token)
        {
            Log.Information($"{nameof(PushToMarket)}");
            var variantWinner = voting.ResponseVariants.FirstOrDefault(x => x.Id == key.Key);
            if(variantWinner is null)
                return;
            
            var organizationWinnerList = JsonConvert.DeserializeObject<OrganizationContent>(variantWinner.OrganizationsContent);
            
            Log.Information($"{nameof(PushToMarket)} organizationWinnerList: {organizationWinnerList.ToJson()}");

            var message = FinishedVotingSuccessMessengerEvent.Create
            (
                Guid.Parse(chat.DocumentId),
                organizationWinnerList.Organizations.Select(x => x.ContractorId).ToArray(),
                likes
            );
            await _bus.Publish(message, token);
        }

        private async Task PushToBot(ChatView chat, ICurrentUser currentUser, KeyValuePair<Guid, int> key, CancellationToken token)
        {
            Log.Information($"{nameof(PushToMarket)}");
            var msg = BuildMessage(chat, key.Key, currentUser);
            Log.Information($"{nameof(PushToMarket)} messsage: {msg.ToJson()}");
            await _bus.Publish(msg, token);
        }

        private static MessengerServerEvent BuildMessage(ChatView chat, Guid winner, ICurrentUser currentUser)
        {
            var messageClient = new MessageClient
            {
                Author = new Author
                (
                    currentUser.Id,
                    currentUser.Login,
                    currentUser.FirstName,
                    currentUser.LastName,
                    currentUser.MiddleName
                ),
                ButtonCommands = Array.Empty<Consumers.Models.ButtonCommand>(),
                ChatId = chat.ChatId,
                ChatTypeMnemonic = chat.Mnemonic,
                Commands = Array.Empty<Consumers.Models.CommandClient>(),
                Content = string.Empty,
                Date = DateTime.UtcNow,
                DocumentId = Guid.Parse(chat.DocumentId),
                File = null,
                MessageId = 1,
                MessageType = MessageTypeClient.Vote,
                ModuleName = Consumers.Models.ModuleName.Market,
                Edited = false,
                Pined = false,
                Readed = false,
                VotingClient = null
            };

            var account = Market.EventBus.RMQ.Shared.Events.Account.Create(
                currentUser.Id,
                currentUser.Login,
                currentUser.FirstName,
                currentUser.LastName,
                currentUser.MiddleName,
                currentUser.OrganizationId,
                null);

            var command = MessageCommand.Create(
                "module.bidCenter.document.bid.action.chooseWinner",
                winner,
                "Выбрать исполнителя",
                "Bot",
                AvailabilityCommand.Open);

            var messageType = Market.EventBus.RMQ.Shared.Events.MessageType.Bot;

            Guid documentId = Guid.Parse(chat.DocumentId);

            var informationChat = InformationChat.Create
            (
                documentId,
                chat.ChatId,
                string.Empty, 
                null, 
                chat.ParentId,
                null,
                chat.DocumentStatusMnemonic, 
                chat.Module,
                chat.ContractorId
            );

            var moduleName = (Market.EventBus.RMQ.Shared.Events.ModuleName)Enum.Parse(typeof(Consumers.Models.ModuleName),
                    messageClient.ModuleName.ToString());

            var header = Header.Create
            (
                account,
                informationChat,
                moduleName,
                messageType,
                command,
                ButtonCommandCollection.Empty
            );

            return MessengerServerEvent.Create(header, account.ConnectionId, string.Empty, messageClient);
        }

        private async Task<ChatView> GetChatById(Guid votingId, CancellationToken token)
        {
            var query = $@"select * from public.'Messages'
                    where 'Content'->@VotingContent@->>@VotingId@ = @{votingId}@"
                    .Replace("'", "\"")
                    .Replace("@", "'");

            var messages = _context.Messages.FromSqlRaw(query);
            var message = await messages.FirstOrDefaultAsync(token);

            return await _context
                .ChatView
                .FirstOrDefaultAsync(x => x.ChatId.Equals(message.ChatId), token);
        }
    }
}
