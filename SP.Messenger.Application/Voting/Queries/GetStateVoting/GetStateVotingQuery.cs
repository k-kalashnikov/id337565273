using MediatR;
using System;

namespace SP.Messenger.Application.Voting.Queries.GetStateVoting
{
    public class GetStateVotingQuery : IRequest<StateVotingDto>
    {
        public GetStateVotingQuery(Guid votingId)
        {
            VotingId = votingId;
        }

        public Guid VotingId { get; }

        public static GetStateVotingQuery Create(Guid votingId)
            => new GetStateVotingQuery(votingId);
    }
}
