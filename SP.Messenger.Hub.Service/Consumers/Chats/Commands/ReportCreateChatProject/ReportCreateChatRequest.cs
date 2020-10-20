using System;
using System.Collections.Generic;

namespace SP.Consumers.Models
{
    public class CreateChatReportProjectRequest
    {
        public string ProjectName { get; set; }
        public string Module { get; set; }
        public Guid ProjectId { get; set; }
        public int DocumentTypeId { get; set; }
        public string ChatTypeMnemonic { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        public ICollection<Account> Accounts { get; set; }
    }
    
    public class Account
    {
        public long id { get; set; }
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public long? organizationId { get; set; }
    }
}