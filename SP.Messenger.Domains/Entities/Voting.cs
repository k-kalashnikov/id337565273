using System;
using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Domains.Entities
{
    public class Voting
    {
        private Voting()
        {
            _responseVariants = new List<ResponseVariant>();
            _votedCollection = new List<VotedBy>();
        }

        public Voting(long createBy, string name):this()
        {
            Id = Guid.NewGuid();
            CreateBy = createBy;
            Name = name;
        }
        public Guid Id { get; private set; }
        public long CreateBy { get; private set; }
        public string Name { get; private set; }
        private List<ResponseVariant> _responseVariants;
        public IReadOnlyCollection<ResponseVariant> ResponseVariants => _responseVariants;

        private List<VotedBy> _votedCollection;
        public IReadOnlyCollection<VotedBy> VotedCollection => _votedCollection;

        public bool IsClosed { get; set; }
        public void AddResponseVariant(string name, Guid? decisionId, IEnumerable<Contractor> organizations)
        {
            _responseVariants.Add(new ResponseVariant(this.Id, name, decisionId, organizations));
        }
        
        public void AddVotedCollection(Guid? responseVariantId, long voted)
        {
            _votedCollection.Add(new VotedBy(this.Id, responseVariantId, voted));
        }

        private void CloseVote()
        {
            IsClosed = true;
        }

        public void Like(long accountId, Guid responseVariantId, bool? like, string comment = null)
        {
            var votedBy = _votedCollection
                .SingleOrDefault(x => x.AccountId == accountId);

            if (votedBy is null)
                throw new InvalidOperationException($"{nameof(Like)} votedBy is null");
            
            _votedCollection.Remove(votedBy);

            if (votedBy.ResponseVariantId == responseVariantId)
            {
                /*Скидываем на Null если пользователь нажал на like дважды.
                 Проверка есть на клиенте запрещающая дважды голосовать,
                 здесь если как дополнительный функционал*/
                votedBy.Set(accountId, null, null);
            }
            else
            {
                votedBy.Set(accountId, responseVariantId, like, comment);
                if (like == false)
                {
                    CloseVote();
                }
            }
            
            _votedCollection.Add(votedBy);
        }
    }
}
