using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using DictionaryDocument = mbt.webapi.Domain.Entities.DictionaryDocument;

namespace mbt.webapi.Endpoints.Dictionaries;

public class GetById : EndpointBaseAsync.WithRequest<string>.WithActionResult<DictionaryDocument>
{
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public GetById(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    [SwaggerOperation(
        Summary = "Get dictionary by Id",
        Description = "Get dictionary by Id",
        OperationId = "Dictionaries.GetById",
        Tags = new[] { "Dictionaries" })]
    [HttpGet("api/dictionaries/{id}")]
    public override async Task<ActionResult<DictionaryDocument>> HandleAsync(
        [FromRoute] string id, CancellationToken cancellationToken = new())
    {
        var dictionaryDocument =
            await _dictionaryRepository.GetAsync(id);

        if (dictionaryDocument == null) return NotFound();

        return dictionaryDocument;
    }
}
