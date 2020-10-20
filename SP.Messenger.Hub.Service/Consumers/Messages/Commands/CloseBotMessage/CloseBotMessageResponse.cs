namespace SP.Consumers.Models
{
    public class CloseBotMessageResponse
    {
        public bool Success { get; set; }
        public static CloseBotMessageResponse Create(bool success)
        => new CloseBotMessageResponse{Success = success};
    }
}