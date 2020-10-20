using System;

namespace SP.Messenger.Domains.Entities
{
    public class Tag
    {
        public string ModuleName { get; set; }
        public string EventBot { get; set; }
        public string ChatTypeMnemonic { get; set; }
        public Guid DocumentId { get; set; }
        public int DocumentStatus { get; set; }
        public Guid StageId { get; set; }
        public int StageStatus { get; set; }
        public Guid Contract { get; set; }
        public Guid ContractAgreement { get; set; }
        public string Communication { get; set; }
        public string Contractor { get; set; }
    }
}
