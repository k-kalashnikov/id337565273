using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Application.DocumentType.Queries
{
    public class DocumentTypeDTO
    {
        public long DocumentTypeId { get; set; }
        public string Name { get; set; }
        public bool IsDisabled { get; set; }

        public static DocumentTypeDTO Create(Domains.Entities.DocumentType model)
            => new DocumentTypeDTO
            {
                DocumentTypeId = model.DocumentTypeId,
                IsDisabled = model.IsDisabled,
                Name = model.Name
            };

        public static DocumentTypeDTO[] Create(List<Domains.Entities.DocumentType> models)
            => models.Select(x => Create(x)).ToArray();
    }
}
