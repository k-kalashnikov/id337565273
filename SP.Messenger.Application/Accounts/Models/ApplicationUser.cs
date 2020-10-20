using System.Collections.Generic;
using System.Linq;

namespace SP.Consumers.Models
{
    public class ApplicationUser
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? OrganizationId { get; set; }

        public static ApplicationUser Create(GetAccountIdentityResponse account)
        {
            return new ApplicationUser
            {
                AccountId = account?.AccountId ?? 0,
                Login = account?.Login,
                FirstName = account?.FirstName,
                LastName = account?.LastName,
                MiddleName = account?.MiddleName,
                OrganizationId = account?.OrganizationId
            };
        }

        public static ApplicationUser[] Create(IEnumerable<GetAccountIdentityResponse> accounts)
            => accounts.Select(Create).ToArray();

    }
}