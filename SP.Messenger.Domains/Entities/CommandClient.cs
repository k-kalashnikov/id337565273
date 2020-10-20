namespace SP.Messenger.Domains.Entities
{
    public class CommandClient
    {
        public string Command { get; set; }
        public string DisplayName { get; set; }
        public string BotMessageType { get; set; }
        public dynamic Value { get; set; }
        public OpenCommand Open { get; set; }
    }
    public enum OpenCommand : byte
    {
        Closed = 0,
        Open = 1
    }
}
