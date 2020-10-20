using FluentValidation;
using SP.Messenger.Domains.Entities;
using System;

namespace SP.Messenger.Application.Messanges.Queries.GetMessagesByOrder
{
    public class GetMessagesByOrderQueryValidator : AbstractValidator<GetMessagesByOrderQuery>
    {
        public GetMessagesByOrderQueryValidator()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).NotNull();
        }
    }
}
