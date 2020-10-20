namespace SP.Messenger.Domains.Entities
{
    public class ChatData
    {
        public long DocumentTypeId { get; set; }
        public string DocumentId { get; set; }
        public string ParentDocumentId { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        public string Module { get; set; }
        public long? ContractorId { get; set; }
    }
}
