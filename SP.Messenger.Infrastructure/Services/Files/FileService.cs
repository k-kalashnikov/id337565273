using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Infrastructure.Services.Infrastructure;

namespace SP.Messenger.Infrastructure.Services.Files
{
    public class FileService : IFileService
    {
        private readonly IFileWriterFactory _fileWriterFactory;

        public FileService(IFileWriterFactory fileWriterFactory)
        {
            _fileWriterFactory = fileWriterFactory ?? throw new ArgumentNullException(nameof(fileWriterFactory));
        }

        public async Task<ProcessingResult<string>> SaveFileMessage(string moduleName, Guid documentId, string fileName, IFormFile formFile)
        {
            //var newFileName = GenerationFileName(fileName);
            
            try
            {
                var file = MessageFile.Create(formFile.FileName, formFile, new []{moduleName, documentId.ToString()});
                return await WriteFileMessageAsync(file);
            }
            catch (Exception ex)
            {
                //Log.Error(exp.ToString());
                return new ProcessingResult<string>(null, new []{WriteFileError.Create("Неизвестная ошибка записи файла в хранилище")});
            }
        }

        public Task<(Stream stream, string fileName)> GetFileMessage(string path)
        {
            throw new NotImplementedException();
        }

        #region Private
        private async Task<ProcessingResult<string>> WriteFileMessageAsync(IFileMessage file)
        {
            return await WriteAsync(file, _fileWriterFactory.CreateMessageWriterOnDisk());
        }
        
        private async Task<ProcessingResult<string>> WriteAsync(IFileMessage file, IFileWriter writer)
        {
            var fullPath = await file.WriteAsync(writer);
            return new ProcessingResult<string>(fullPath);
        }

        private static string GenerationFileName(string fileName)
        {
            var str = Path.GetRandomFileName();
            str = str.Replace(".", "");

            return $"{str}{Path.GetExtension(fileName)}";
        }

        #endregion
    }
}