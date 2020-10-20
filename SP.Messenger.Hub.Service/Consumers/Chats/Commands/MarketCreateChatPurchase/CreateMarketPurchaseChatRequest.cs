using System;

namespace SP.Consumers.Models
{
    public class CreateMarketPurchaseChatRequest
    {
        public Guid PurchaseId { get; }
        public string PurchaseName { get; }
        public long OrganizationId { get; }
        public string ChatTypeMnemonic { get; }
        public int DocumentTypeId { get; }
        public string DocumentStatusMnemonic { get; }
        public string Module { get; }
        public Guid? ParentDocumentId { get; }
        public AccountForMessengerDTO[] Accounts { get; }

        public CreateMarketPurchaseChatRequest(Guid purchaseId, string purchaseName, long organizationId,
            string chatTypeMnemonic, int documentTypeId, string documentStatusMnemonic, string module, Guid? parentDocumentId, AccountForMessengerDTO[] accounts)
        {
            PurchaseId = purchaseId;
            PurchaseName = purchaseName;
            OrganizationId = organizationId;
            ChatTypeMnemonic = chatTypeMnemonic;
            DocumentTypeId = documentTypeId;
            DocumentStatusMnemonic = documentStatusMnemonic;
            Module = module;
            ParentDocumentId = parentDocumentId;
            Accounts = accounts;
        }

    }
}
