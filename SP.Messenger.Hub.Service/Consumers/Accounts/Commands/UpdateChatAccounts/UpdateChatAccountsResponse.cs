namespace SP.Consumers.Models
{
    public class UpdateChatAccountsResponse
    {
        public bool Success { get; set; }

        public static UpdateChatAccountsResponse Create(bool success)
            => new UpdateChatAccountsResponse
            {
                Success = success
            };
    }
}