using FluentValidation;

namespace SP.Messenger.Application.Messages.Command.Remove
{
    public class RemoveMessageCommandValidator : AbstractValidator<RemoveMessageCommand>
    {
        public RemoveMessageCommandValidator()
        {
            RuleFor(x => x.AccountId).NotNull().NotEqual(0);
            RuleFor(x => x.MessageId).NotNull().NotEqual(0);
        }
    }
}