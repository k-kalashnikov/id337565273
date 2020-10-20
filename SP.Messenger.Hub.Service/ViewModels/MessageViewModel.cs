using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SP.Messenger.Hub.Service.ViewModels
{
    public class MessageViewModel
    {
        [FromForm(Name = "module")]
        public int Module { get; set; }
//        [FromForm(Name="documentType")]
//        public int DocumentType { get; set; }
        [FromForm(Name="documentId")]
        public Guid DocumentId { get; set; }
        [FromForm(Name="chatId")]
        public long ChatId { get; set; }
        [FromForm(Name="content")]
        public string Content { get; set; }
        [FromForm(Name="file")]
        public IFormFile File { get; set; }
    }
}