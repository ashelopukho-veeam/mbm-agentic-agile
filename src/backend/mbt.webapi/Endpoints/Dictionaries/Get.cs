using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Dictionaries;

public class Get : EndpointBaseAsync.WithoutRequest.WithActionResult<List<GetDictionariesResponse>>
{
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public Get(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    [SwaggerOperation(
        Summary = "List dictionaries",
        Description = "List dictionaries",
        OperationId = "Dictionaries.Get",
        Tags = new[] { "Dictionaries" })]
    [HttpGet("api/dictionaries")]
    public override async Task<ActionResult<List<GetDictionariesResponse>>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var dictionaryDocuments =
            await _dictionaryRepository.GetAsync();

        var response = dictionaryDocuments.Select(GetDictionariesResponse.FromDictionaryDocument).ToList();

        return response;
    }
}
