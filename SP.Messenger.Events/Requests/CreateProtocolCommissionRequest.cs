using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Messenger.Events.Requests
{
  public class CreateProtocolCommissionRequest
  {
    public Guid ProtocolId { get; set; }

    public IEnumerable<long> Accounts { get; set; }

  }
}
