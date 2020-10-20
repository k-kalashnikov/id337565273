using MediatR;
using SP.Messenger.Common.Implementations;
using System;

namespace SP.Messenger.Application.Voting.Commands.SetVote
{
    public class SetVoteCommand : IRequest<ProcessingResult<bool>>
    {
        public SetVoteCommand(Guid voteId, Guid responseVariant, bool? like, string comment = null)
        {
            VoteId = voteId;
            ResponseVariant = responseVariant;
            Like = like;
            Comment = comment;
        }

        public Guid VoteId { get; }
        public Guid ResponseVariant { get; }
        public bool? Like { get; }
        public string Comment { get; }

        public static SetVoteCommand Create(Guid voteId, Guid responseVariant, bool? like, string comment = null)
            => new SetVoteCommand(voteId, responseVariant, like, comment);
    }
}
