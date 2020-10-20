using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;

namespace SP.Messenger.Application.Messages.Validators
{
    public static class MessageValidator
    {
        public static class RemoveRule
        {
            public static ProcessingResult<bool> RemoveMessageCheck(Message message, long accountInitiator)
            {
                if(message is null)
                    return new ProcessingResult<bool>(false, new []
                    {
                        new SimpleResponseError("Сообщение не было найдено"), 
                    });
                
                if (message.AccountId != accountInitiator)
                    return new ProcessingResult<bool>(false, new []
                    {
                        new SimpleResponseError("Нельзя удалять чужие сообщения"), 
                    });
                return new ProcessingResult<bool>(true);
            }
        }
        
        public static class Entity
        {
            public static ProcessingResult<bool> CheckNull(Message message)
            {
                if(message is null)
                    return new ProcessingResult<bool>(false, new []
                    {
                        new SimpleResponseError("Сообщение не было найдено"), 
                    });
                return new ProcessingResult<bool>(true);
            }
        }
    }
}