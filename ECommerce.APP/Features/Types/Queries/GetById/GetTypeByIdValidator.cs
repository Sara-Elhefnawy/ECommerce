using FluentValidation;

namespace ECommerce.APP.Features.Types.Queries.GetById;

public sealed class GetTypeByIdValidator : AbstractValidator<GetTypeByIdQuery>
{
    public GetTypeByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID must not be empty.")
            .WithErrorCode("Type.Id.Required");
    }
}
