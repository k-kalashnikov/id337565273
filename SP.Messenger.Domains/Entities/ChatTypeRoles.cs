using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Domains.Entities
{
    public class ChatTypeRoles
    {
        protected ChatTypeRoles() 
        {
        }

        public ChatTypeRoles(int chatTypeId, string roleMnemonic)
        {
            ChatTypeId = chatTypeId;
            RoleMnemonic = roleMnemonic;
        }

        public long Id { get; private set; }
        public int ChatTypeId { get; private set; }
        public ChatType ChatType { get; private set; }
        public string RoleMnemonic { get; private set; }
    }
}
