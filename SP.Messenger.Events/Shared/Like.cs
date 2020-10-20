using System;
using System.Collections.Generic;

namespace SP.Messenger.Client.Shared
{
    public class Likes
    {
        public Likes(string variantName, Guid? decisionId, IEnumerable<VotingContractor> contractors, int like, IEnumerable<LikeAccount> accountIds, bool? isLike, string comment = null)
        {
            VariantName = variantName;
            DecisionId = decisionId;
            Contractors = contractors;
            Like = like;
            AccountIds = accountIds;
            IsLike = isLike;
            Comment = comment;
        }

        public string VariantName { get; }
        public Guid? DecisionId { get; }
        public IEnumerable<VotingContractor> Contractors { get; }
        public int Like { get; }
        public IEnumerable<LikeAccount> AccountIds { get; }
        public bool? IsLike { get; }
        public string Comment { get; }
    }

    public class LikeAccount 
    {
        public long AccountId { get; set; }
        public bool? IsLike { get; set; }
    }

}