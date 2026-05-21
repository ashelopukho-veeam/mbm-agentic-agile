using FluentValidation;
using JetBrains.Annotations;
using mbt.webapi.BuiltIn;
using mbt.webapi.Domain;

namespace mbt.webapi.Endpoints.Transfers.dto;

[PublicAPI]
public class CreateTransferRequest : IWithPaidMediaData
{
    public string Title { get; set; }
    public string FromBudgetId { get; set; }
    public string ToBudgetId { get; set; }
    public string FromQuarter { get; set; }
    public string ToQuarter { get; set; }
    public double Amount { get; set; }
    public string Comment { get; set; }

    public string GlobalRegion { get; set; }
    public string SubRegion { get; set; }
    public string GlobalProgram { get; set; }
    public string Team { get; set; }
    public string AdService { get; set; }
    public string Management { get; set; }
    public string ExecutionTeam { get; set; }
    public string ContentType { get; set; }
}

public class CreateTransferRequestValidator : AbstractValidator<CreateTransferRequest>
{
    public CreateTransferRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(ValidationRulesConstants.TitleMaxLength);
        RuleFor(x => x.FromBudgetId).IsObjectId();
        RuleFor(x => x.ToBudgetId).IsObjectId();
        RuleFor(x => x.FromQuarter)
            .NotEmpty()
            .Matches(ValidationRulesConstants.QuarterRegexPattern)
            .WithMessage(ErrorMessages.InvalidQuarter);
        RuleFor(x => x.ToQuarter)
            .NotEmpty()
            .Matches(ValidationRulesConstants.QuarterRegexPattern)
            .WithMessage(ErrorMessages.InvalidQuarter);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Comment)
            .NotEmpty().MaximumLength(ValidationRulesConstants.MaxCommentLength);
    }
}
