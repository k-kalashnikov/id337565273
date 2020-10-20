using System;
using FluentValidation;

namespace SP.Messenger.Application.Files.Queries.GetFilesByDocument
{
    public class GetFilesByDocumentQueryValidator : AbstractValidator<GetFilesByDocumentQuery>
    {
        public GetFilesByDocumentQueryValidator()
        {
            RuleFor(x => x.DocumentId).NotNull().NotEqual(Guid.Empty);
        }
    }
}