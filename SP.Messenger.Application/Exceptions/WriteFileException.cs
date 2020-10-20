using System;

namespace SP.Messenger.Application.Exceptions
{
    public class WriteFileException: Exception
    {
        public WriteFileException(string name, object key)
            : base($"File {name} don`t write {key}")
        {

        }
        public WriteFileException(string message)
            : base(message)
        {

        }
    }
}