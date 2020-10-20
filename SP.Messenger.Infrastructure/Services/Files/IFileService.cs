using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Infrastructure.Services.Files
{
    public interface IFileService
    {
        Task<ProcessingResult<string>> SaveFileMessage(string moduleName, Guid documentId, string fileName, IFormFile file);
        Task<(Stream stream, string fileName)> GetFileMessage(string path);
    }
}