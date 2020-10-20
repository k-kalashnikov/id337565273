using MediatR;
using SP.Messenger.Application.ChatTypeRoles.Models;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SP.Messenger.Application.ChatTypeRoles.Queries
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, ChatTypeRolesDto>
    {
        private readonly MessengerDbContext _context;

        public GetRolesQueryHandler(MessengerDbContext context)
        {
            _context = context;
        }

        public async Task<ChatTypeRolesDto> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = from chatType in _context.ChatTypes.AsNoTracking()
                        join chatTypeRole in _context.ChatTypeRoles.AsNoTracking() on chatType.ChatTypeId equals chatTypeRole.ChatTypeId
                        where chatType.Mnemonic == request.ChatTypeMnemonic
                        select chatTypeRole;
           
            return ChatTypeRolesDto.Create(await roles.ToArrayAsync(cancellationToken));
        }
    }
}
