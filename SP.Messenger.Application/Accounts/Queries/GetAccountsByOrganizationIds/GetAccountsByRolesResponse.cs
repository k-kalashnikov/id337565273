namespace SP.Consumers.Models
{
    public class GetAccountsByRolesResponse
    {
        public long AccountId { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public long? OrganizationId { get; set; }
    }
}
