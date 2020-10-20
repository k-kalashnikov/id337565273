namespace SP.Consumers.Models
{
    public class GetAccountsByOrganizationResponse
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? OrganizationId { get; set; }

        public static GetAccountsByOrganizationResponse Create(long accountId, string login, 
            string firstName, string lastName, string middleName)
            => new GetAccountsByOrganizationResponse
            {
                AccountId = accountId,
                Login = login,
                FirstName = firstName,
                LastName = lastName,
                MiddleName = middleName                
            };
    }
}