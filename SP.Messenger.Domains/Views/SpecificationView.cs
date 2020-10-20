using System;

namespace SP.Messenger.Domains.Entities
{
    public class SpecificationView
    {
        public long ChatId { get; set; }
        public ChatType ChatType { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }

        public static string View => "specificationchatsview";
    }
}
