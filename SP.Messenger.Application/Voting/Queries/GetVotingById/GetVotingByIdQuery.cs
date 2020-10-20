using MediatR;
using SP.Messenger.Application.Voting.Models;
using System;

namespace SP.Messenger.Application.Voting.Queries
{
    public class GetVotingByIdQuery : IRequest<VotingDto>
    {
        public GetVotingByIdQuery(Guid votingId)
        {
            VotingId = votingId;
        }

        public Guid VotingId { get; }

        public static GetVotingByIdQuery Create(Guid votingId)
            => new GetVotingByIdQuery(votingId);
    }
}
