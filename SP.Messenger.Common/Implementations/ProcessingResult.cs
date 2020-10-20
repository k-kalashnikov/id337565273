using SP.Messenger.Common.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace SP.Messenger.Common.Implementations
{
    public class ProcessingResult<T> : IProcessingResult<T>
    {
        public ProcessingResult(T result, IEnumerable<IError> errors = null)
        {
            Result = result;
            Errors = errors ?? Enumerable.Empty<IError>();
        }

        public virtual T Result { get; }

        public virtual IEnumerable<IError> Errors { get; }
    }

    public class ProcessingResult<T1, T2> : IProcessingResult<T1> where T2 : class, IError
    {
        public ProcessingResult(T1 result, IEnumerable<T2> errors = null)
        {
            Result = result;
            Errors = errors ?? Enumerable.Empty<T2>();
        }

        public virtual T1 Result { get; }

        public virtual IEnumerable<IError> Errors { get; }


    }
}
