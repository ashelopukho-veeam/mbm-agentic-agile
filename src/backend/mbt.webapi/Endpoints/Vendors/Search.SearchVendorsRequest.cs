using FluentValidation;

namespace mbt.webapi.Endpoints.Vendors;

public class SearchVendorsRequest
{
    public const string Route = "api/vendors/search";

    public string Search { get; set; }

    public int Limit { get; set; } = 10;
}

public class SearchVendorsRequestValidator : AbstractValidator<SearchVendorsRequest>
{
    public SearchVendorsRequestValidator()
    {
        RuleFor(x => x.Search).NotEmpty();
        RuleFor(x => x.Limit).InclusiveBetween(1, 1000);
    }
}
