using System.Collections.Generic;
using MediatR;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsChatById
{
    public class GetAccountsChatByIdQuery : IRequest<long[]>
    {
        public long ChatId { get; }

        public GetAccountsChatByIdQuery(long chatId)
        {
            ChatId = chatId;
        }
        
        public static GetAccountsChatByIdQuery Create(long chatId)
            => new GetAccountsChatByIdQuery(chatId);
    }
}