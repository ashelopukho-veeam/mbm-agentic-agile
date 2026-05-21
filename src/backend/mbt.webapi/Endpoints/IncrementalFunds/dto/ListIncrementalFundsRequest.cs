using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.Utils;

namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class ListIncrementalFundsRequest
{
    public string Plan { get; set; }
    public int Year { get; set; }
}


public class ListIncrementalFundsRequestValidator : AbstractValidator<ListIncrementalFundsRequest>
{
    public ListIncrementalFundsRequestValidator()
    {
        RuleFor(x => x.Plan)
            .Must(p => QuarterUtils.Quarters.Contains(p))
            .When(p => !string.IsNullOrWhiteSpace(p.Plan))
            .WithMessage(ErrorMessages.InvalidQuarter);

        RuleFor(x => x.Year).NotEmpty().GreaterThanOrEqualTo(2000);
    }
}
