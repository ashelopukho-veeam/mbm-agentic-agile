using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Configuration.Validation;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Dictionaries;

public class RemoveItemFromDictionary : EndpointBaseAsync.WithRequest<RemoveItemFromDictionaryRequest>
    .WithActionResult<DictionaryDto>
{
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public RemoveItemFromDictionary(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("/api/admin/dictionaries/removeItem")]
    [SwaggerOperation(
        Summary = "Remove item from dictionary",
        Description = "Remove item from dictionary",
        OperationId = "Dictionaries.RemoveItem",
        Tags = new[] { "Dictionaries" })]
    public override async Task<ActionResult<DictionaryDto>> HandleAsync(RemoveItemFromDictionaryRequest request,
        CancellationToken cancellationToken = new())
    {
        var dictionaryDocument = await _dictionaryRepository.GetAsync(request.Id);
        if (dictionaryDocument == null) return NotFound();

        if (!dictionaryDocument.Items.Contains(request.Title))
        {
            throw new ApiException("Item does not exist in dictionary");
        }
        else
        {
            dictionaryDocument.Items.Remove(request.Title);
            await _dictionaryRepository.UpdateAsync(dictionaryDocument);
        }

        var resultDto = new DictionaryDto(dictionaryDocument.Id, dictionaryDocument.Title,
            dictionaryDocument.InternalName,
            dictionaryDocument.Items.ToArray());

        return resultDto;
    }
}

public record RemoveItemFromDictionaryRequest(string Id, string Title);
