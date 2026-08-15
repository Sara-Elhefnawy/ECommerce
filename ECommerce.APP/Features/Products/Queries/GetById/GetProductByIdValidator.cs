using FluentValidation;

namespace ECommerce.APP.Features.Products.Queries.GetById;

public sealed class GetProductByIdValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("ID must not be empty.")
            .WithErrorCode("Product.Id.Required");
    }
}
