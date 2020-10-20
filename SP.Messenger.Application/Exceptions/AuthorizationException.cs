using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException(CRUDOperationException operation)
             : base($"Access is denied of operation to {nameof(operation)}")
        {

        }
    }

    public enum CRUDOperationException
    {
        Create,
        Read,
        Update,
        Delete
    }
}
