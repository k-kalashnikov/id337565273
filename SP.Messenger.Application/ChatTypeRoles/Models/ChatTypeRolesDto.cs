using System;
using System.Collections.Generic;
using System.Text;
using SP.Messenger.Domains.Entities;
using System.Linq;

namespace SP.Messenger.Application.ChatTypeRoles.Models
{
    public class ChatTypeRolesDto
    {
        public ChatTypeRolesDto(string[] roles)
        {
            Roles = roles;
        }

        public string[] Roles { get; private set; }

        public static ChatTypeRolesDto Create(IEnumerable<SP.Messenger.Domains.Entities.ChatTypeRoles> models)
            => new ChatTypeRolesDto(models.Select(x=>x.RoleMnemonic).ToArray());

    }
}
