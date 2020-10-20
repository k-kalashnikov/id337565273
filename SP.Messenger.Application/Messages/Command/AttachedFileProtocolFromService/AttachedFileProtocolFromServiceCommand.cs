using System;
using MediatR;

namespace SP.Messenger.Application.Messages.Command.AttachedFileProtocolFromService
{
    public class AttachedFileProtocolFromServiceCommand : IRequest<Unit>
    {
        public Guid ProtocolId { get; }
        public string Link { get; }

        public AttachedFileProtocolFromServiceCommand(Guid protocolId, string link)
        {
            ProtocolId = protocolId;
            Link = link;
        }
    }
}