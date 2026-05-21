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

public class AddItemToDictionary : EndpointBaseAsync.WithRequest<AddItemToDictionaryRequest>.WithActionResult<
    DictionaryDto>
{
    private readonly IDbBaseRepository<DictionaryDocument> _dictionaryRepository;

    public AddItemToDictionary(IDbBaseRepository<DictionaryDocument> dictionaryRepository)
    {
        _dictionaryRepository = dictionaryRepository;
    }

    [Authorize(Roles = AppRoles.AdminPolicy)]
    [HttpPost("/api/admin/dictionaries/addItem")]
    [SwaggerOperation(
        Summary = "Add item to dictionary",
        Description = "Add item to dictionary",
        OperationId = "Dictionaries.AddItem",
        Tags = new[] { "Dictionaries" })]
    public override async Task<ActionResult<DictionaryDto>> HandleAsync(AddItemToDictionaryRequest request,
        CancellationToken cancellationToken = new())
    {
        var dictionaryDocument = await _dictionaryRepository.GetAsync(request.Id);
        if (dictionaryDocument == null) return NotFound();

        if (!dictionaryDocument.Items.Contains(request.Title))
        {
            dictionaryDocument.Items.Add(request.Title);
            await _dictionaryRepository.UpdateAsync(dictionaryDocument);
        }
        else
        {
            throw new ApiException("Item already exists in dictionary");
        }

        var resultDto = new DictionaryDto(dictionaryDocument.Id, dictionaryDocument.Title,
            dictionaryDocument.InternalName,
            dictionaryDocument.Items.ToArray());

        return resultDto;
    }
}

public record AddItemToDictionaryRequest(string Id, string Title);
