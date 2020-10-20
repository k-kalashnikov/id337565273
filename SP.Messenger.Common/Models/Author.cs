using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Common.Models
{
    public class Author
    {
        [JsonProperty("foreignKey")]
        public int ForeignKey { get; set; }
        [JsonProperty("login")]
        public string login { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
