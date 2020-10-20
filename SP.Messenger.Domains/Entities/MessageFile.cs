using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Domains.Entities
{
    public class MessageFile
    {
        public string ContentType { get; set; }
        public string Filename { get; set; }
        public string Url { get; set; }
        public int Length { get; set; }
        public string Extension { get; set; }
    }
}
