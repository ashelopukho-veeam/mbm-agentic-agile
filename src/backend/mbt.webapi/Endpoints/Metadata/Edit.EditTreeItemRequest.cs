using FluentValidation;

namespace mbt.webapi.Endpoints.Metadata;

public class EditTreeItemRequest
{
    public string Id { get; set; }

    public string Title { get; set; }
    public string Value { get; set; }
}


public class EditTreeItemRequestValidator : AbstractValidator<EditTreeItemRequest>
{
    public EditTreeItemRequestValidator()
    {
        RuleFor(x => x.Id).IsObjectId();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Value).NotEmpty();
    }
}
