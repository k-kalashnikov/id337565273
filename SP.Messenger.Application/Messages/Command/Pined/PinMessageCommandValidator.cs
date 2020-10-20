using FluentValidation;

namespace SP.Messenger.Application.Messages.Command.Pined
{
    public class PinMessageCommandValidator : AbstractValidator<PinMessageCommand>
    {
        public PinMessageCommandValidator()
        {
            RuleFor(x => x.MessageId).NotNull().NotEqual(0);
        }
    }
}