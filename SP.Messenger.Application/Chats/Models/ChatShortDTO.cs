using System;
using System.Collections.Generic;
using System.Linq;
using SP.Market.Core.Extension;
using SP.Messenger.Application.Chats.Models;
using SP.Messenger.Domains.Entities;

namespace SP.Consumers.Models
{
    public class ChatShortDTO
    {
        public long ChatId { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Mnemonic { get; set; }
        public bool IsDisabled { get; set; }
        public Guid DocumentId { get; set; }
        public Guid? ParentDocumentId { get; set; }
        public string DocumentStatus { get; set; }
        public string Module { get; set; }
        public long? ContractorId { get; set; }
        public IEnumerable<string> Accounts { get; private set; }

        public static ChatShortDTO Create(Chat model)
        {
            var chatData = model.Data.FromJson<ChatData>();
            Guid.TryParse(chatData?.DocumentId, out var documentId);
            Guid.TryParse(chatData?.ParentDocumentId, out var parentDocumentId);
            var contractorId = chatData?.ContractorId;
            
            return new ChatShortDTO
            {
                ChatId = model.ChatId,
                ParentId = model.ParentId,
                Name = model.Name,
                Mnemonic = ChatTypeDTO.Create(model.ChatType)?.Mnemonic,
                IsDisabled = model.IsDisabled,
                DocumentId = documentId,
                ParentDocumentId = parentDocumentId,
                DocumentStatus = chatData?.DocumentStatusMnemonic,
                Module = chatData?.Module,
                ContractorId = contractorId,
                Accounts = model.Accounts?.Select(x=>x.Account?.Login)
            };
        } 
    }
}