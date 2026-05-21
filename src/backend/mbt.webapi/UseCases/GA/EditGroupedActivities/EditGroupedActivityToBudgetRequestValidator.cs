using FluentValidation;
using mbt.webapi.BuiltIn;

namespace mbt.webapi.UseCases.GA.EditGroupedActivities;

public class EditGroupedActivityToBudgetRequestValidator : AbstractValidator<EditGroupedActivityToBudgetRequest>
{
    public EditGroupedActivityToBudgetRequestValidator()
    {
        RuleFor(x => x.Id).IsObjectId();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ValidationRulesConstants.TitleMaxLength);
        RuleFor(x => x.Quarter).InclusiveBetween(1, 4);
        RuleFor(x => x.PlannedAmount).GreaterThan(0);
        RuleFor(x => x.PlannedSponsorship).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LocalCurrency).NotEmpty();
        RuleFor(x => x.GlobalRegion).NotEmpty();
        RuleFor(x => x.SubRegion).NotEmpty();
        RuleFor(x => x.Segments)
            .IsValidPercentageDistribution();
        RuleFor(x => x.Campaigns)
            .IsValidPercentageDistribution();
        RuleFor(x => x.Comment)
            .MaximumLength(ValidationRulesConstants.MaxCommentLength);
    }
}
