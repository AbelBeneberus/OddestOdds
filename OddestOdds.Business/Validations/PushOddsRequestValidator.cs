using FluentValidation;
using OddestOdds.Common.Models;

namespace OddestOdds.Business.Validations;

public class PushOddsRequestValidator : AbstractValidator<PushOddsRequest>
{
    public PushOddsRequestValidator()
    {
        RuleFor(x => x.MarketSelectionIds)
            .Must((request, ids) => ValidateMarketSelectionIds(request.PushAll, ids))
            .WithMessage("MarketSelectionIds cannot be empty when PushAll is false.");
    }

    private bool ValidateMarketSelectionIds(bool pushAll, IEnumerable<Guid> ids)
    {
        if (pushAll)
        {
            return true;
        }

        return ids != null && ids.Any();
    }
}