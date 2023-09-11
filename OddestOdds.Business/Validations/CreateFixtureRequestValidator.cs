using FluentValidation;
using OddestOdds.Common.Models;

namespace OddestOdds.Business.Validations;

public class CreateFixtureRequestValidator : AbstractValidator<CreateFixtureRequest>
{
    public CreateFixtureRequestValidator()
    {
        RuleFor(cfr => cfr.AwayTeam)
            .NotEmpty()
            .NotNull()
            .WithMessage("The Away Team name can not be null or empty.");

        RuleFor(cfr => cfr.HomeTeam)
            .NotEmpty()
            .NotNull()
            .WithMessage("The Home Team name can not be null or empty.");

        RuleFor(cfr => cfr.HomeTeam)
            .NotEmpty()
            .NotNull()
            .WithMessage("The Home Team name can not be null or empty.");
    }
}