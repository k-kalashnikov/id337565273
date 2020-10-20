using SP.Consumers.Models;
using System;
using System.Collections.Generic;

namespace SP.Messenger.Application.Voting.Queries.GetStateVoting
{
    public class StateVotingDto
    {
        public StateVotingDto(long chatId, long messageId, Guid documentId, Guid votingId, IEnumerable<StateVotingObject> stateVotingObjects)
        {
            ChatId = chatId;
            MessageId = messageId;
            DocumentId = documentId;
            VotingId = votingId;
            StateVotingObjects = stateVotingObjects;
        }

        public long ChatId { get; }
        public long MessageId { get; }
        public Guid DocumentId { get; }
        public Guid VotingId { get; }
        public IEnumerable<StateVotingObject> StateVotingObjects { get; }
    }

    public class StateVotingObject
    {
        public StateVotingObject(Guid? variantId, IEnumerable<VotedAccounts> accounts, bool? like)
        {
            VariantId = variantId;
            Accounts = accounts;
        }

        public Guid? VariantId { get; }
        public IEnumerable<VotedAccounts> Accounts { get; }
    }
}

