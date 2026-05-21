using FluentValidation;
using JetBrains.Annotations;

namespace mbt.webapi.Endpoints.Tasks;

[PublicAPI]
public class ResolveTaskRequest
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string Comment { get; set; }
}

public class ResolveTaskRequestValidator : AbstractValidator<ResolveTaskRequest>
{
    public ResolveTaskRequestValidator()
    {
        RuleFor(x => x.Id).IsObjectId();
        RuleFor(x => x.Status).NotEmpty();
    }
}
