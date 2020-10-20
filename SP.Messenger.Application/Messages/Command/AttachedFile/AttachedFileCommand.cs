using MediatR;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Messages.Command.AttachedFile
{
    public class AttachedFileCommand : IRequest<ProcessingResult<long>>
    {
        public AttachedFileCommand(long chatId, string link)
        {
            ChatId = chatId;
            Link = link;
        }

        public long ChatId { get; }
        public string Link { get; }

        public static AttachedFileCommand Create(long chatId, string link)
            => new AttachedFileCommand(chatId, link);
    }
}
