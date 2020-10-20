using System;

namespace SP.Messenger.Domains.Entities
{
    public class VotedBy
    {
        private VotedBy()
        {

        }
        public VotedBy(Guid votingId, Guid? responseVariantId, long votedId)
        {
            Id = Guid.NewGuid();
            VotingId = votingId;
            ResponseVariantId = responseVariantId;
            AccountId = votedId;
        }
        public Guid Id { get; private set; }
        
        public Guid VotingId { get; private set; }
        public Voting Voting { get; private set; }
        
        public Guid? ResponseVariantId { get; private set; }
        public ResponseVariant ResponseVariant { get; private set; }

        public long AccountId { get; private set; }
        public Account Account { get; private set; }

        public bool? IsLike { get; private set; }
        public string Comment { get; private set; }

        public void Set(long votedId, Guid? responseVariantId, bool? like, string comment = null)
        {
            ResponseVariantId = responseVariantId;
            AccountId = votedId;
            IsLike = like;

            if (comment != null)
            {
                Comment = comment;
            }
        }
    }
}
