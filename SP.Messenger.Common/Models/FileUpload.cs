using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Common.Models
{
    public class FileUpload
    {
        [JsonProperty("contentType")]
        public string ContentType { get; set; }
        [JsonProperty("fileName")]
        public string Filename { get; set; }
        [JsonProperty("lenght")]
        public long Lenght { get; set; }
        [JsonProperty("extension")]
        public string Extension { get; set; }
        [JsonProperty("dataBase64")]
        public string Data { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("messageTimeId")]
        public string MessageTimeId { get; set; }
        [JsonProperty("roleId")]
        public int RoleId { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}
