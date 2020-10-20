using System;
using FluentValidation;

namespace SP.Messenger.Application.Messages.Command.SaveSystemMessage
{
    public class SaveSystemMessageCommandValidator : AbstractValidator<SaveSystemMessageCommand>
    {
        public SaveSystemMessageCommandValidator()
        {
            RuleFor(x => x.Message).NotNull().NotEmpty();
            RuleFor(x => x.DocumentId).NotNull().NotEqual(Guid.Empty);
            RuleFor(x => x.AccountId).GreaterThan(0);
        }
    }
}