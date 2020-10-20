namespace SP.Messenger.Common.Settings
{
    public class Settings
    {
        public Logging Logging { get; set; }
        public ConnectionString ConnectionString { get; set; }
        public RMQClient RMQClient { get; set; }
        public string AllowedHosts { get; set; }
        public FileServer FileServer { get; set; }
        public Templates Templates { get; set; }
    }
}
