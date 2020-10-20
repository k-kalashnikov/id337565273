namespace SP.Consumers.Models
{
    public class GetAccountsByOrganizationRequest
    {
        public long[] OrganizationIds { get; set; }

        public static GetAccountsByOrganizationRequest Create(long[] organizationIds)
            => new GetAccountsByOrganizationRequest
            {
                OrganizationIds = organizationIds
            };
    }
}