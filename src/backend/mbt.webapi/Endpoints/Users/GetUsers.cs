using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Services;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;

namespace mbt.webapi.Endpoints.Users;

public class GetUsers : EndpointBaseAsync.WithRequest<string>.WithActionResult<PagedResult<UserProfile>>
{
    private readonly IUserService _userService;

    public GetUsers(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("api/users")]
    [SwaggerOperation(
        Summary = "Get users",
        Description = "Get users (paged). Page size: 5",
        OperationId = "Users.List",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<PagedResult<UserProfile>>> HandleAsync([FromQuery] string p,
        CancellationToken cancellationToken = new())
    {
        int.TryParse(p, out var page);

        var result = await _userService.GetPaged(page > 0 ? page : 1);
        return result;
    }
}
