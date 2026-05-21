using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain;

namespace mbt.webapi.Endpoints.IncrementalFunds.dto;

[PublicAPI]
public class CreateIncrementalFundRequest : IWithPaidMediaData
{
    public string Title { get; set; }
    public string ToBudgetId { get; set; }
    public int ToQuarter { get; set; }
    public double Amount { get; set; }
    public string Description { get; set; }

    #region Paid media fields

    public string GlobalRegion { get; set; }
    public string SubRegion { get; set; }
    public string GlobalProgram { get; set; }
    public string Team { get; set; }
    public string AdService { get; set; }
    public string Management { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }

    #endregion Paid media fields
}

public class CreateIncrementalFundRequestValidator : AbstractValidator<CreateIncrementalFundRequest>
{
    public CreateIncrementalFundRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ValidationRulesConstants.TitleMaxLength);
        RuleFor(x => x.ToBudgetId).IsObjectId();
        RuleFor(x => x.ToQuarter).InclusiveBetween(1, 4);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Description).NotEmpty()
            .MaximumLength(ValidationRulesConstants.MaxCommentLength);
    }
}
