namespace SP.Consumers.Models
{
    public class BidCreateInviteChatsResponse
    {
        public BidCreateInviteChatsResponse(bool success, string[] errors)
        {
            Success = success;
            Errors = errors;
        }
        public bool Success { get; set; }
        public string[] Errors { get; set; }
        
        public static BidCreateInviteChatsResponse Create(bool success, string[] errors )
        => new BidCreateInviteChatsResponse(success, errors);
    }
}