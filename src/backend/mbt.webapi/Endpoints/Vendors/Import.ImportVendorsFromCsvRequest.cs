using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Endpoints.Vendors;

public class ImportVendorsFromCsvRequest
{
    public IFormFile FormFile { get; set; }
}

public class ImportVendorsFromCsvRequestValidator : AbstractValidator<ImportVendorsFromCsvRequest>
{
    public ImportVendorsFromCsvRequestValidator()
    {
        RuleFor(x => x.FormFile).NotNull().WithMessage("File is required");
    }
}
