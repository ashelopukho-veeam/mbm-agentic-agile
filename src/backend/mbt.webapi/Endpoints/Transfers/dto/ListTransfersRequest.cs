
using System.Linq;
using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.Utils;

namespace mbt.webapi.Endpoints.Transfers.dto;

public class ListTransfersRequest
{
    public string Plan { get; set; }

    public int Year { get; set; }
}

[UsedImplicitly]
public class ListTransfersRequestValidator : AbstractValidator<ListTransfersRequest>
{
    public ListTransfersRequestValidator()
    {
        RuleFor(x => x.Plan)
            .Must(p => QuarterUtils.Quarters.Contains(p))
            .When(p => !string.IsNullOrWhiteSpace(p.Plan))
            .WithMessage(ErrorMessages.InvalidQuarter);

        RuleFor(x => x.Year).NotEmpty().GreaterThanOrEqualTo(2000);
    }
}
