using System;
using System.Collections.Generic;

namespace SP.Consumers.Models
{
    public class CreateChatReportQuestionRequest
    {
        public string QuestionName { get; set; }
        public Guid QuestionId { get; set; }
        public string Module { get; set; }
        public Guid ProjectId { get; set; }
        public int DocumentTypeId { get; set; }
        public string ChatTypeMnemonic { get; set; }
        public string DocumentStatusMnemonic { get; set; }
        public ICollection<Account> Accounts { get; set; }
        
        public static CreateChatReportQuestionRequest Create(string questionName, Guid questionId, string module, Guid projectId, int documentTypeId,
            string chatTypeMnemonic, string documentStatusMnemonic, ICollection<Account> accounts)
            => new CreateChatReportQuestionRequest
            {
                QuestionName = questionName,
                QuestionId = questionId,
                Module = module,
                ProjectId = projectId,
                DocumentTypeId = documentTypeId,
                ChatTypeMnemonic = chatTypeMnemonic,
                DocumentStatusMnemonic = documentStatusMnemonic,
                Accounts = accounts
            };
    }
}