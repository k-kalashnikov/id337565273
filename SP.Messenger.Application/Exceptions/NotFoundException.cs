using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string name, object key)
            : base($"Entity {name} with key {key} was not found")
        {

        }
    }
}
