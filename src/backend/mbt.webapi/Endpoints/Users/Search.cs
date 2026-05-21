using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Swashbuckle.AspNetCore.Annotations;

namespace mbt.webapi.Endpoints.Users;

public class Search : EndpointBaseAsync.WithRequest<string>.WithActionResult<List<UserProfile>>
{
    private readonly IUserService _userService;
    private readonly GraphServiceClient _graphServiceClient;

    public Search(IUserService userService, GraphServiceClient graphServiceClient)
    {
        _userService = userService;
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet("api/users/search")]
    [SwaggerOperation(
        Summary = "Search users",
        Description = "Search users",
        OperationId = "Users.Search",
        Tags = new[] { "Users" })]
    public override async Task<ActionResult<List<UserProfile>>> HandleAsync([FromQuery] string search,
        CancellationToken cancellationToken = new())
    {
        var limit = 10;
        var result = new List<UserProfile>();
        var graphResult = await GetUsersFromGraph(search, limit);

        if (graphResult is not { Count: > 0 }) return result;

        foreach (var gUser in graphResult)
        {
            result.Add(gUser);
            await _userService.EnsureUser(gUser);
        }

        return result;
    }

    private async Task<List<UserProfile>> GetUsersFromGraph(string search, int limit)
    {
        // list graph users and exclude disabled accounts
        var graphResult = await _graphServiceClient.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Search = $"\"displayName:{search}\"";
            // requestConfiguration.QueryParameters.Search = "\"displayName:room\"";
            // exclude disabled accounts
            requestConfiguration.QueryParameters.Filter += "accountEnabled eq true";
            // return only users with Member type Member (not Guest)
            requestConfiguration.QueryParameters.Filter += " and userType eq 'Member'";
            requestConfiguration.Headers.Add("ConsistencyLevel", "eventual");

            requestConfiguration.QueryParameters.Top = limit;
        }, CancellationToken.None);

        if (graphResult is not { Value: not null }) return null;

        return graphResult.Value.Select(r => new UserProfile
        {
            DisplayName = r.DisplayName,
            Id = r.Id,
            Email = r.Mail
        }).ToList();
    }
}
