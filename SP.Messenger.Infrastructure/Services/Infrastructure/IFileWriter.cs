using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Infrastructure
{
    public interface IFileWriter
    {
        Task<string> WriterFileAsync(IFileMessage file);
    }
}