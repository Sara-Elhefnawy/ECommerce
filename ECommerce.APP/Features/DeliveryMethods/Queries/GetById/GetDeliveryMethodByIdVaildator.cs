using FluentValidation;

namespace ECommerce.APP.Features.DeliveryMethods.Queries.GetById;

public sealed class GetDeliveryMethodByIdVaildator : AbstractValidator<GetDeliveryMethodByIdQuery>
{
    public GetDeliveryMethodByIdVaildator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Delivery method ID must not be empty.")
            .WithErrorCode("DeliveryMethod.Id.Required");
    }
}
