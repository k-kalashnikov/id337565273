using FluentValidation;

namespace SP.Messenger.Application.Messages.Command.Save
{
    public class SaveMessageCommandValidator :AbstractValidator<SaveMessageCommand>
    {
        public SaveMessageCommandValidator()
        {
            RuleFor(x => x.Message).NotNull();
            RuleFor(x => x.Message.AccountId).NotNull().NotEqual(0);
        }
    }
}