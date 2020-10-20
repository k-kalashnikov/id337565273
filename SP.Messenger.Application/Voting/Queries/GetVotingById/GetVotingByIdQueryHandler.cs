using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Application.Voting.Models;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Voting.Queries
{
    public class GetVotingByIdQueryHandler : IRequestHandler<GetVotingByIdQuery, VotingDto>
    {
        private readonly MessengerDbContext _context;

        public GetVotingByIdQueryHandler(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<VotingDto> Handle(GetVotingByIdQuery request, CancellationToken cancellationToken)
        {
            var voting = await _context.Votings
                .Include(x => x.ResponseVariants)
                .Include(x => x.VotedCollection).ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.Id == request.VotingId, cancellationToken);

            return VotingDto.Create(voting);
        }
    }
}
