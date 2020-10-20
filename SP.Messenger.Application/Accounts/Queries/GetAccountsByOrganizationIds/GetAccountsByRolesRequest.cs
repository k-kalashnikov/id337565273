using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Consumers.Models
{
    public class GetAccountsByRolesRequest
    {
        public GetAccountsByRolesRequest(bool includeSuperUsers, IEnumerable<OrganizationRoles> organizations)
        {
            IncludeSuperUsers = includeSuperUsers;
            Organizations = organizations;
        }

        public bool IncludeSuperUsers { get; }
        public IEnumerable<OrganizationRoles> Organizations { get; }

        public class OrganizationRoles
        {
            public string[] Roles { get; set; }
            public long OrganizationId { get; set; }
        }
    }
}
