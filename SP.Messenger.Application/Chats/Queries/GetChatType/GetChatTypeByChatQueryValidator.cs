using FluentValidation;

namespace SP.Messenger.Application.Chats.Queries.GetChatType
{
    public class GetChatTypeByChatQueryValidator : AbstractValidator<GetChatTypeByChatQuery>
    {
        public GetChatTypeByChatQueryValidator()
        {
            RuleFor(x => x.ChatId).NotNull().NotEqual(0);
        }
    }
}
