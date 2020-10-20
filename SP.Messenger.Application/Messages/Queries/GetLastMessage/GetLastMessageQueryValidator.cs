using FluentValidation;
using System;

namespace SP.Messenger.Application.Chats.Queries.GetLastMessage
{
    public class GetLastMessageQueryValidator : AbstractValidator<GetLastMessageQuery>
    {
        public GetLastMessageQueryValidator()
        {
            RuleFor(x => x.StockOrderId).NotNull().NotEqual(Guid.Empty);
        }
    }
}
