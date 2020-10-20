using System.Collections.Generic;

namespace SP.Consumers.Models
{
    public class GetPanelButtonsResponse
    {
        public IEnumerable<ButtonCommand> ChatButtons { get; set; }
    }
}