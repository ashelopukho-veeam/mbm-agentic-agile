using FluentValidation;
using mbt.webapi.BuiltIn;

namespace mbt.webapi.Endpoints.CommonConfig;

public class SetCommonConfigRequest
{
    public bool AllowCreateTransfers { get; set; }
    public bool AllowCreateReforecasts { get; set; }
    public bool NotificationBarMessageEnabled { get; set; }
    public string NotificationBarMessage { get; set; }
}

public class SetCommonConfigRequestValidator : AbstractValidator<SetCommonConfigRequest>
{
    public SetCommonConfigRequestValidator()
    {
        RuleFor(x => x.NotificationBarMessage).NotNull();
    }
}
