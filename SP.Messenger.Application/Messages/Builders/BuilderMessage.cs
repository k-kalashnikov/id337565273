using SP.Messenger.Domains.Entities;
using System;
using System.Linq;
using SP.Consumers.Models;
using SP.Messenger.Common.Extensions;
using Entity = SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Messages.Builders
{
    public class BuilderMessage
    {
        private readonly Message _message;
        public BuilderMessage(long chatId, long accountId, long messageTypeId, MessageClient content, long? recipientId = null)
        {
            var commandEvent = content.Commands.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Command));

            var openCommand = Entity.OpenCommand.Closed;
            if(commandEvent!=null)
                openCommand = (Entity.OpenCommand)Enum.Parse(typeof(Consumers.Models.AvailabilityCommand), 
                    commandEvent.Open.ToString());
            
            var contentMessage = new Entity.ContentMessage
            {
                Content = (content.MessageType == MessageTypeClient.Bot || content.MessageType == MessageTypeClient.System) 
                ? commandEvent?.DisplayName 
                : content.Content,
                CommandClient = new Entity.CommandClient
                {
                    Command = commandEvent?.Command,
                    DisplayName = commandEvent?.DisplayName,
                    Open = commandEvent == null ? Entity.OpenCommand.Closed : openCommand,
                    BotMessageType = commandEvent?.BotMessageType,
                    Value = commandEvent?.Value,
                    
                },
                File = new Entity.MessageFile
                {
                    Extension = content.File?.Extension,
                    Filename = content.File?.Filename,
                    Url = content.File?.Url,
                    Length = content.File?.Lenght?? 0,
                    ContentType = content.File?.ContentType
                },
                Tags = new Entity.Tag
                {
                    ModuleName = content.ModuleName.ToString(),
                    DocumentId = content.DocumentId,
                    ChatTypeMnemonic = content.ChatTypeMnemonic,
                    EventBot = string.Empty
                },
                VotingContent = new Domains.Entities.VotingContent
                {
                    VotingId = content?.VotingClient?.VotingId ?? Guid.Empty
                }
            };
            
            _message = new Message(chatId, accountId,  false,messageTypeId, contentMessage.ToJson(), content.Date, recipientId);
        }

        public Message Build() => _message;
    }
}
