namespace SP.Messenger.Domains.Entities
{
    public class ContentMessage
    {
        public string Content { get; set; }
        public MessageFile File { get; set; }
        public Tag Tags { get; set; }
        public CommandClient CommandClient {get;set;}
        public VotingContent VotingContent { get; set; }
    }
}
