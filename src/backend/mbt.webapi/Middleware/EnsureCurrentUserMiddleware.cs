using System.Threading.Tasks;
using mbt.webapi.Domain.Entities;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace mbt.webapi.Middleware;

public class EnsureCurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IUserService _userService;
    private readonly ICurrentUserContext _currentUserContext;

    public EnsureCurrentUserMiddleware(RequestDelegate next,
        IUserService userService,
        ICurrentUserContext currentUserContext
    )
    {
        _next = next;
        _currentUserContext = currentUserContext;
        _userService = userService;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!string.IsNullOrWhiteSpace(_currentUserContext.UserId))
            await EnsureUser(_currentUserContext.UserId);

        await _next(httpContext);
    }

    private async Task EnsureUser(string userId)
    {
        var user = await _userService.GetAsync(userId);
        if (user != null) return;

        var contextUser = new UserProfile()
        {
            Id = _currentUserContext.UserId,
            DisplayName = _currentUserContext.UserName,
            Email = _currentUserContext.Email,
        };

        await _userService.EnsureUser(contextUser);
    }
}
