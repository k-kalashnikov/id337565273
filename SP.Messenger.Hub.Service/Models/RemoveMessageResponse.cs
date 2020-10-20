namespace SP.Consumers.Models
{
    public class RemoveMessageResponse
    {
        public bool Success { get; set; }
        public string[] Errors { get; set; }
    }
}