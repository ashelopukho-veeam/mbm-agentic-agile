using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using CsvHelper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using VendorsDocument = mbt.webapi.Domain.Entities.VendorsDocument;

namespace mbt.webapi.Endpoints.Vendors;

public class Export : EndpointBaseAsync.WithoutRequest.WithActionResult
{
    private readonly IDbBaseRepository<VendorsDocument> _vendorsRepository;

    public Export(IDbBaseRepository<VendorsDocument> vendorsRepository)
    {
        _vendorsRepository = vendorsRepository;
    }

    [HttpPost("api/vendors/export")]
    [SwaggerOperation(
        Summary = "Export vendors",
        Description = "Export vendors to CSV file",
        OperationId = "Export.Vendors",
        Tags = new[] { "Vendors" })]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new())
    {
        var vendors = await _vendorsRepository.GetAsync();

        var vendorsData = ToByteArray(vendors);
        var memoryStream = new MemoryStream(vendorsData);

        var result = new FileStreamResult(memoryStream, "text/csv")
        { FileDownloadName = "vendors.csv" };

        return result;
    }

    private static byte[] ToByteArray(IEnumerable<VendorsDocument> records)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);

        using var csvWriter = new CsvWriter(streamWriter, CsvConstants.DefaultCsvConfiguration);
        csvWriter.Context.RegisterClassMap<VendorsDocumentCsvMap>();

        csvWriter.WriteRecords(records);
        streamWriter.Flush();
        return memoryStream.ToArray();
    }
}
