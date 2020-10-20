using System;

namespace SP.Consumers.Models
{
    public class GetPanelButtonsRequest
    {
        public long AccountId { get; set; }
        public Guid DocumentId { get; set; }
        public long ChatId { get; set; }
        public ModuleName ModuleName { get; set; }

        public static GetPanelButtonsRequest Create(long accountId, Guid documentId, long chatId, ModuleName module)
            => new GetPanelButtonsRequest
            {
                AccountId = accountId,
                DocumentId = documentId,
                ChatId = chatId,
                ModuleName = module
            };
    }
}