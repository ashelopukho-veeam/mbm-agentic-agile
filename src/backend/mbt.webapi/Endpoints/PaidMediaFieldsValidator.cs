using FluentValidation;
using mbt.webapi.Domain;

namespace mbt.webapi.Endpoints;

public class PaidMediaFieldsValidator : AbstractValidator<IWithPaidMediaData>
{
    public PaidMediaFieldsValidator()
    {
        RuleFor(x => x.GlobalRegion).NotEmpty();
        RuleFor(x => x.SubRegion).NotEmpty();
        RuleFor(x => x.GlobalProgram).NotEmpty();
        RuleFor(x => x.Team).NotEmpty();
        RuleFor(x => x.AdService).NotEmpty();
        RuleFor(x => x.Management).NotEmpty();
        RuleFor(x => x.ExecutionTeam).NotEmpty();
        RuleFor(x => x.ContentType).NotEmpty();
    }
}
