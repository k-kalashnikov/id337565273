using System;
using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.SaveSystemMessage
{
    public class SaveSystemMessageCommand : IRequest<ProcessingResult<bool>>
    {
        public SaveSystemMessageCommand(long accountId, Guid documentId, string mnemonic, string message)
        {
            AccountId = accountId;
            DocumentId = documentId;
            Mnemonic = mnemonic;
            Message = message;
        }

        public long AccountId { get; }
        public Guid DocumentId { get; }
        public string Mnemonic { get; } = "module.bidCenter.chat.common";
        public string Message { get; }

        public static SaveSystemMessageCommand Create(long accountId, Guid documentId, string mnemonic, string message)
            => new SaveSystemMessageCommand(accountId, documentId, mnemonic, message);
    }
}
