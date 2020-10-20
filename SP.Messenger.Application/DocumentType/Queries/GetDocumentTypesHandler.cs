using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Messenger.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.DocumentType.Queries
{
    public class GetDocumentTypesHandler : IRequestHandler<GetDocumentTypes, DocumentTypeDTO[]>
    {
        private readonly MessengerDbContext _context;
        public GetDocumentTypesHandler(MessengerDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<DocumentTypeDTO[]> Handle(GetDocumentTypes request, CancellationToken cancellationToken)
        {
            var documentTypes = await _context.DocumentTypes.AsNoTracking().ToListAsync(cancellationToken);
            return DocumentTypeDTO.Create(documentTypes);
        }
    }
}
