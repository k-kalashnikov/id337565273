using System;
using System.Collections.Generic;
using SP.Messenger.Client.Shared;

namespace SP.Messenger.Client.Events
{
    public class FinishedVotingByDislikeMessengerEvent
    {
        public FinishedVotingByDislikeMessengerEvent(Guid documentId, long[] organizations, IEnumerable<Likes> likes)
        {
            DocumentId = documentId;
            Organizations = organizations;
            Likes = likes;
        }

        public Guid DocumentId { get; }
        public long[] Organizations { get; }
        public IEnumerable<Likes> Likes { get; }
        
        public static FinishedVotingByDislikeMessengerEvent Create(Guid documentId, long[] organizations, IEnumerable<Likes> likes)
            => new FinishedVotingByDislikeMessengerEvent(documentId, organizations, likes);
    }
}