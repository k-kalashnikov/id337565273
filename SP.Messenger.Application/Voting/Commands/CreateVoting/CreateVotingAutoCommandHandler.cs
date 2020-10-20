using MediatR;
using Serilog;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Voting.Commands.CreateVoting
{
    public class CreateVotingAutoCommandHandler : IRequestHandler<CreateVotingAutoCommand, ProcessingResult<Guid>>
    {
        private readonly MessengerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateVotingAutoCommandHandler(MessengerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task<ProcessingResult<Guid>> Handle(CreateVotingAutoCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser == null) 
            {
                Log.Error($"{nameof(CreateVotingAutoCommandHandler)} - currentUser is null");
            }

            var voting = new Domains.Entities.Voting(currentUser?.Id ?? 1, request.Name);

            voting = (await _context.Votings.AddAsync(voting)).Entity;


            foreach (var item in request.ResponseVariants)
            {
                await _context.ResponseVariants.AddAsync(new Domains.Entities.ResponseVariant(
                  voting.Id,
                  item.VariantName,
                  item.DecisionId,
                  item.Contractors.Select(x => new Domains.Entities.Contractor
                  {
                    ContractorId = x.ContractorId,
                    ContractorName = x.ContractorName,
                    DefermentDeviation = x.DefermentDeviation,
                    DefermentPayment = x.DefermentPayment,
                    DeviationBestPrice = x.DeviationBestPrice,
                    PriceOffer = x.PriceOffer,
                    Term = x.Term,
                    TermDeviation = x.TermDeviation,
                    PercentDifferentByPurchase = x.PercentDifferentByPurchase,
                    PercentDifferentByBestContractorOffer = x.PercentDifferentByBestContractorOffer,
                    TermLimit = x.TermLimit,
                    ProtocolNumber = x.ProtocolNumber
                  })));
            }

            foreach (var item in request.Accounts)
            {
                await _context.VotedBy.AddAsync(new Domains.Entities.VotedBy(voting.Id, null, item));
            }

            //foreach (var item in request.Accounts)
            //    voting.AddVotedCollection(null, item);

            await _context.SaveChangesAsync(cancellationToken);

            return new ProcessingResult<Guid>(voting.Id);
        }
    }
}
