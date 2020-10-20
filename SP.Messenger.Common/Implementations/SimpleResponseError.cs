using SP.Messenger.Common.Contracts;

namespace SP.Messenger.Common.Implementations
{
    public class SimpleResponseError : IError
    {
        public SimpleResponseError(string messageError)
        {
            Message = messageError;
        }
        public string Message { get; }
    }
}