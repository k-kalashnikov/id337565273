using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Application.Chats.Builders
{
    public class BuilderDocument
    {
        private readonly Document document;
        public BuilderDocument(Guid documentId, long documentTypeId)
        {
            document = new Document();
            document.DocumentTypeId = documentTypeId;
            document.DocumentId = documentId;
        }

        public Document Build() => document;
    }
}
