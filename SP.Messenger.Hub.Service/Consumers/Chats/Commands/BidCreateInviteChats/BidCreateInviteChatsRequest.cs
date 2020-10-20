namespace SP.Consumers.Models
{
    public class BidCreateInviteChatsRequest
    {
        public BidCreateInviteChatsRequest(ChatPerformer[] chatPerformers)
        {
            ChatPerformers = chatPerformers;
        }
        public ChatPerformer[] ChatPerformers { get; }
    }
}