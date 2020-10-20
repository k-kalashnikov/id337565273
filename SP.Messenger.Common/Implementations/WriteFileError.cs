using SP.Messenger.Common.Contracts;

namespace SP.Messenger.Common.Implementations
{
    public class WriteFileError : IError
    {
        public WriteFileError(string message)
        {
            Message = message;
        }
        public string Message { get; }
        public  static WriteFileError Create(string message)
            => new WriteFileError(message);
    }
}