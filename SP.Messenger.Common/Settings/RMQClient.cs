using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Messenger.Common.Settings
{
    public class RMQClient
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Exchange { get; set; }
        public string Queque { get; set; }
        public int RetryCount { get; set; }
    }
}
