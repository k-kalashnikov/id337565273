namespace SP.Consumers.Models
{
    public class RequestAccountsByOrganization2
    {
        public long? OrganizationId { get; set; }
        
        public static RequestAccountsByOrganization2 Create(long? organization)
            => new RequestAccountsByOrganization2 {OrganizationId = organization};
    }
}
