using System;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public class FileWriterFactory : IFileWriterFactory
    {
        private readonly PlatformID _platformOs;
        private readonly IWritingSettings _settings;

        public FileWriterFactory(PlatformID platformOs, IWritingSettings settings)
        {
            _platformOs = platformOs;
            _settings = settings;
        }
        
        public IFileWriter CreateMessageWriterOnDisk()
        {
            return FileWriterOnDisk.Create(_settings.UploadMessagesFilePath,
                DirectoryHelper.GetDirectorySeparatorChar(_platformOs));
        }
    }
}