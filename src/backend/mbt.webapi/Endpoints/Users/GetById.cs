using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Users;

public class GetById : EndpointBaseAsync.WithRequest<string>.WithActionResult<UserProfile>
{
    private readonly IUserService _userService;
    private readonly GraphServiceClient _graphServiceClient;

    public GetById(IUserService userService, GraphServiceClient graphServiceClient)
    {
        _userService = userService;
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet("api/users/{userId}")]
    [SwaggerOperation(
        Summary = "Get user by Id",
        Description = "Get user by Id",
        OperationId = "Users.GetById",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<UserProfile>> HandleAsync([FromRoute] string userId,
        CancellationToken cancellationToken = new())
    {
        var userProfile = await _userService.GetAsync(userId);

        if (userProfile != null) return userProfile;

        // try to ensure from graph
        var user = await _graphServiceClient.Users[userId].GetAsync(cancellationToken: cancellationToken);

        if (user == null) return NotFound();

        userProfile = new UserProfile
        {
            DisplayName = user.DisplayName,
            Id = user.Id,
            Email = user.Mail
        };

        await _userService.EnsureUser(userProfile);

        return userProfile;
    }
}
