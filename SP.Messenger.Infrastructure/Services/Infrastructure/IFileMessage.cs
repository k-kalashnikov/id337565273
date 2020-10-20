using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SP.Messenger.Infrastructure.Services.Files;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public interface IFileMessage
    {
        string[] RelativeDirectories { get; }

        string Name { get; }

        string Extenstion { get; }

        IFormFile Data { get; }

        Task<string> WriteAsync(IFileWriter writer);
    }
}