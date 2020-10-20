using SP.Messenger.Common.Contracts;

namespace SP.Messenger.Common.Implementations
{
    public class ResponseError : IError
    {
        public ResponseError(string errorMessage, Error errorType = Error.other, int errorStatusCode = 400)
        {
            ErrorType = errorType;
            ErrorStatusCode = errorStatusCode;
            ErrorMessage = errorMessage;
        }

        public Error ErrorType { get; }
        public int ErrorStatusCode { get; }
        public string ErrorMessage { get; }
        public string Message => string.Format("Ошибка {0} : {1}", ErrorStatusCode, ErrorMessage);
    }

    public enum Error
    {
        None = 0,
        Server = 1,
        Proptocol,
        other,
        DataType
    }
}
