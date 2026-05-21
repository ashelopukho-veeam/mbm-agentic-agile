using FluentValidation;

namespace mbt.webapi.Endpoints.Metadata;

public class CreateTreeItemRequest
{
    public string Title { get; set; }
    public string ParentId { get; set; }
    public string Value { get; set; }
}

public class CreateTreeItemRequestValidator : AbstractValidator<CreateTreeItemRequest>
{
    public CreateTreeItemRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.ParentId).IsObjectId()
            .Unless(x => string.IsNullOrEmpty(x.ParentId));
        RuleFor(x => x.Value).NotEmpty();
    }
}
