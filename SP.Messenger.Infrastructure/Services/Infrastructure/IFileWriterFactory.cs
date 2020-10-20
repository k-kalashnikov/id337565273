using System;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public interface IFileWriterFactory
    {
        IFileWriter CreateMessageWriterOnDisk();
    }
}