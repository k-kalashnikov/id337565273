namespace SP.Consumers.Models
{
    public class GetLastMessagesResponse
    {
        public string Content { get; set; }
        
        public static GetLastMessagesResponse Create(string content)
        => new GetLastMessagesResponse
        {
            Content = content
        }; 
        
    }
}