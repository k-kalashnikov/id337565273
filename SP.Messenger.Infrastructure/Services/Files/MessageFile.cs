using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SP.Messenger.Infrastructure.Services.Infrastructure;

namespace SP.Messenger.Infrastructure.Services.Files
{
    public class MessageFile : IFileMessage
    {
        public MessageFile(string fileName, IFormFile file, params string[] directories)
        {
            RelativeDirectories = directories ?? Array.Empty<string>();
            Name = fileName;
            Extenstion = Path.GetExtension(fileName);
            Data = file;
        }

        public static MessageFile Create(string fileName, IFormFile file, params string[] directories)
            => new MessageFile(fileName, file, directories);

        public string[] RelativeDirectories { get; }

        public string Name { get; }

        public string Extenstion { get;  }

        public IFormFile Data { get;  }

        public async Task<string> WriteAsync(IFileWriter writer)
        {
            return await writer.WriterFileAsync(this);
        }
    }
}