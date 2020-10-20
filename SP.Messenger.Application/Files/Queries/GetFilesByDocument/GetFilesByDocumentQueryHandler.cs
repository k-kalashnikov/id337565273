using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SP.Messenger.Application.Files.Models;
using SP.Messenger.Common.Settings;
using SP.Messenger.Persistence;

namespace SP.Messenger.Application.Files.Queries.GetFilesByDocument
{
    public class GetFilesByDocumentQueryHandler : IRequestHandler<GetFilesByDocumentQuery, FileShortDto[]>
    {
        private readonly MessengerDbContext _context;
        private readonly IOptions<Settings> _options;
        public GetFilesByDocumentQueryHandler(MessengerDbContext context, IOptions<Settings> options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task<FileShortDto[]> Handle(GetFilesByDocumentQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.MessageFilesView
                .Where(x => x.DocumentId==request.DocumentId.ToString())
                .Include(x=>x.Account)
                .ToArrayAsync(cancellationToken);
            return FileShortDto.Create(data, _options.Value);
        }
    }
}