using System;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Domains.Views
{
    public class MessageFilesView
    {
        public long MessageId { get; set; }
        public DateTime CreateDate { get; set; }
        public long ChatId { get; set; }
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public string DocumentId { get; set; }
    
        public string Url { get; set; }
        public string Length { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        
        public static string View => "messagefilesview";
    }
}