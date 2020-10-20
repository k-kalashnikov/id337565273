using System.Collections.Generic;

namespace SP.Messenger.Common.Contracts
{
    public interface IProcessingResult<T>
    {
        T Result { get; }
        IEnumerable<IError> Errors { get; }
    }
}
