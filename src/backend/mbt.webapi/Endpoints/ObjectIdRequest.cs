using FluentValidation;
using JetBrains.Annotations;

namespace mbt.webapi.Endpoints;

[PublicAPI]
public class ObjectIdRequest
{
    public string Id { get; set; }
}

[UsedImplicitly]
public class ObjectIdRequestValidator : ObjectIdRequestBaseValidator<ObjectIdRequest>
{
}

[UsedImplicitly]
public class ObjectIdRequestBaseValidator<T> : AbstractValidator<T> where T : ObjectIdRequest
{
    public ObjectIdRequestBaseValidator()
    {
        RuleFor(v => v.Id).IsObjectId();
    }
}
