using System.Collections.Generic;

namespace SP.Consumers.Models
{
    public class PanelButtonCommandsRequest
    {
        public string ConnectionId { get; set; }
        public IEnumerable<ButtonCommand> ChatButtons { get; set; }
    }
}