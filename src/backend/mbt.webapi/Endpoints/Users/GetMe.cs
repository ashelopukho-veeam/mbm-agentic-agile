using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using UserProfile = mbt.webapi.Domain.Entities.UserProfile;

namespace mbt.webapi.Endpoints.Users;

public class GetMe : EndpointBaseAsync.WithoutRequest.WithActionResult<UserProfile>
{
    private readonly IUserService _userService;
    private readonly ICurrentUserContext _currentUserContext;

    public GetMe(IUserService userService, ICurrentUserContext currentUserContext)
    {
        _userService = userService;
        _currentUserContext = currentUserContext;
    }

    [Authorize(Roles = AppRoles.ViewPolicy)]
    [HttpGet("api/users/me")]
    [SwaggerOperation(
        Summary = "Get current user",
        Description = "Get current user",
        OperationId = "Users.GetMe",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<UserProfile>> HandleAsync(
        CancellationToken cancellationToken = new())
    {
        var contextUser = new UserProfile()
        {
            Id = _currentUserContext.UserId,
            DisplayName = _currentUserContext.UserName,
            Email = _currentUserContext.Email,
        };

        var result = await _userService.EnsureUser(contextUser);

        return result;
    }
}
