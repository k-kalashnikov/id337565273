using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SP.Messenger.Hub.Service.ViewModels
{
    //For BIA
    public class SystemMessage
    {
        [FromForm(Name="login")]
        public string Login { get; set; }
        [FromForm(Name="documentType")]
        public int DocumentType { get; set; }
        [FromForm(Name="documentId")]
        public Guid DocumentId { get; set; }
        [FromForm(Name="content")]
        public string Content { get; set; }
        [FromForm(Name="file")]
        public IFormFile File { get; set; }

        [JsonIgnore]
        public Market.EventBus.RMQ.Shared.Events.ModuleName Module { get; set; }
    }
}