using FluentValidation;
using OddestOdds.Common.Models;

namespace OddestOdds.Business.Validations;

public class CreateOddRequestValidator : AbstractValidator<CreateOddRequest>
{
    public CreateOddRequestValidator()
    {
        RuleFor(cor => cor.OddValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("OddValue can't be negative");

        RuleFor(cor => cor.SelectionName)
            .NotEmpty()
            .NotNull()
            .WithMessage("Name can not be null or empty.");
    }
}