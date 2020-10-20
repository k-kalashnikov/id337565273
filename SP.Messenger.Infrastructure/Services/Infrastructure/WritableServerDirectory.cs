namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public class WritableServerDirectory : IWritingSettings
    {
        public WritableServerDirectory(string path)
        {
            UploadMessagesFilePath = path;
        }
        public string UploadMessagesFilePath { get; set; }
        
        public static WritableServerDirectory Create(string path)
        => new WritableServerDirectory(path);
    }
}