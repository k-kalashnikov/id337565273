using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Domains.Views
{
    public class StateVotingView
    {
        public Guid? ResponseVariantId { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public Guid VotingId { get; set; }
        public bool? IsLike { get; set; }
        public string Comment { get; set; }

        public static string View => "statevotingview";
    }
}
