namespace SP.Consumers.Models
{
    public class GetAccountIdentityRequest
    {
        public string Email { get; set; }
        public static GetAccountIdentityRequest Create(string email)
            => new GetAccountIdentityRequest { Email = email };
    }
}