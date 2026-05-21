using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using CsvHelper;
using FluentValidation;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VendorsDocument = mbt.webapi.Domain.Entities.VendorsDocument;

namespace mbt.webapi.Endpoints.Vendors;

public class Import : EndpointBaseAsync.WithRequest<ImportVendorsFromCsvRequest>.WithoutResult
{
    private readonly IDbBaseRepository<VendorsDocument> _vendorsRepository;
    private readonly IValidator<ImportVendorsFromCsvRequest> _validator;

    public Import(IDbBaseRepository<VendorsDocument> vendorsRepository, IValidator<ImportVendorsFromCsvRequest> validator)
    {
        _vendorsRepository = vendorsRepository;
        _validator = validator;
    }

    [HttpPost("api/vendors/import")]
    [SwaggerOperation(
        Summary = "Import vendors",
        Description = "Import vendors from CSV file",
        OperationId = "Import.Vendors",
        Tags = new[] { "Vendors" })]
    public override async Task<ActionResult> HandleAsync([FromForm] ImportVendorsFromCsvRequest request,
        CancellationToken cancellationToken = new())
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        using var reader = new StreamReader(request.FormFile.OpenReadStream());
        using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
        csv.Context.RegisterClassMap<VendorsDocumentCsvMap>();
        var records = csv.GetRecords<VendorsDocument>().ToList();

        if (records.Count == 0)
        {
            throw new ApiException("No records found in the file");
        }

        await _vendorsRepository.ClearAsync();
        await _vendorsRepository.CreateManyAsync(records);

        return Ok();
    }
}
