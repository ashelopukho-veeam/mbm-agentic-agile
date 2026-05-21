using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using CsvHelper;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using PaidMediaDetails = mbt.webapi.Domain.Entities.PaidMediaDetails;

namespace mbt.webapi.Endpoints.PaidMediaSet;

public class Export : EndpointBaseAsync.WithRequest<ObjectIdRequest>.WithActionResult
{
    private readonly IPaidMediaSetService _paidMediaSetService;

    public Export(IPaidMediaSetService paidMediaSetService)
    {
        _paidMediaSetService = paidMediaSetService;
    }

    [Authorize(Roles = AppRoles.PaidMediaRolePolicy)]
    [HttpGet("api/paidMediaSet/{Id}/export")]
    [SwaggerOperation(
        Summary = "Export Paid Media Set details",
        Description = "Export Paid Media Set details",
        OperationId = "PaidMediaSet.ExportDetails",
        Tags = new[] { "PaidMediaSet" })]
    public override async Task<ActionResult> HandleAsync([FromRoute] ObjectIdRequest request,
        CancellationToken cancellationToken = new())
    {
        var paidMediaSet = await _paidMediaSetService.GetAsync(request.Id);
        if (paidMediaSet == null) return NotFound();

        return ExportPaidMediaDetails(paidMediaSet);
    }

    private FileStreamResult ExportPaidMediaDetails(Domain.Entities.PaidMediaSet paidMediaSet)
    {
        var result = WriteCsvToMemory(paidMediaSet.Details);
        var memoryStream = new MemoryStream(result);
        return new FileStreamResult(memoryStream, "text/csv")
            { FileDownloadName = paidMediaSet.Title.ToLower().Replace(" ", "_") + ".csv" };
    }


    private static byte[] WriteCsvToMemory(IEnumerable<PaidMediaDetails> records)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);

        using var csvWriter = new CsvWriter(streamWriter, CsvConstants.DefaultCsvConfiguration);

        csvWriter.WriteRecords(records);
        streamWriter.Flush();
        return memoryStream.ToArray();
    }
}
