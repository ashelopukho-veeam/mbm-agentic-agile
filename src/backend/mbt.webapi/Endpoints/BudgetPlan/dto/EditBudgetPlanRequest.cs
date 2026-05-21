using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.Domain.Entities;

namespace mbt.webapi.Endpoints.BudgetPlan.dto;

[PublicAPI]
public class EditBudgetPlanRequest
{
    public string BudgetPlanId { get; set; }

    public double Q1 { get; set; }
    public double Q2 { get; set; }
    public double Q3 { get; set; }
    public double Q4 { get; set; }

    public List<TitleNumberValuePair> Segments { get; set; }
    public List<TitleNumberValuePair> Campaigns { get; set; }
}

[UsedImplicitly]
public class EditBudgetPlanRequestValidator : AbstractValidator<EditBudgetPlanRequest>
{
    public EditBudgetPlanRequestValidator()
    {
        RuleFor(v => v.Q1)
            .GreaterThanOrEqualTo(0);
        RuleFor(v => v.Q2)
            .GreaterThanOrEqualTo(0);
        RuleFor(v => v.Q3)
            .GreaterThanOrEqualTo(0);
        RuleFor(v => v.Q4)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.BudgetPlanId)
            .Length(BuiltIn.BuiltInConstants.ObjectIdLength)
            .NotEmpty();

        // check all segments & campaigns values > 0
        RuleForEach(t => t.Segments)
            .ChildRules(segment =>
            {
                segment.RuleFor(x => x.Value).GreaterThan(0);
                segment.RuleFor(x => x.Title).NotEmpty();
            })
            .WithMessage("Each value should be > 0");

        RuleForEach(t => t.Campaigns)
            .ChildRules(campaign =>
            {
                campaign.RuleFor(x => x.Value).GreaterThan(0);
                campaign.RuleFor(x => x.Title).NotEmpty();
            })
            .WithMessage("Each value should be > 0");

        // check segments & campaigns sum == 100%
        RuleFor(x => x.Segments)
            .NotNull()
            .NotEmpty()
            .Must(coll => Convert.ToInt32(coll.Sum(item => item.Value)) == 100)
            .WithMessage("The sum of values should be equal to 100%");

        RuleFor(x => x.Campaigns)
            .NotNull()
            .NotEmpty()
            .Must(coll => Convert.ToInt32(coll.Sum(item => item.Value)) == 100)
            .WithMessage("The sum of values should be equal to 100%");
    }
}
