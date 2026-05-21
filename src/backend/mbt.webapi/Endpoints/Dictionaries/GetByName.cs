using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using DictionaryDocument = mbt.webapi.Domain.Entities.DictionaryDocument;

namespace mbt.webapi.Endpoints.Dictionaries;

public class GetByName : EndpointBaseAsync.WithRequest<string>.WithActionResult<DictionaryDocument>
{
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public GetByName(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    [SwaggerOperation(
        Summary = "Get dictionary by internal name",
        Description = "Get dictionary by internal name",
        OperationId = "Dictionaries.GetByInternalName",
        Tags = new[] { "Dictionaries" })]
    [HttpGet("api/dictionaries/getByInternalName/{dictionaryInternalName}")]
    public override async Task<ActionResult<DictionaryDocument>> HandleAsync(
        [FromRoute] string dictionaryInternalName, CancellationToken cancellationToken = new())
    {
        var dictionaryDocument =
            await _dictionaryRepository.FindOneAsync(d => d.InternalName == dictionaryInternalName);

        if (dictionaryDocument == null) return NotFound();

        return dictionaryDocument;
    }
}
