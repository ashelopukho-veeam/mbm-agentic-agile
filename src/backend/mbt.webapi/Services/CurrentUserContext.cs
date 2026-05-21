using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using mbt.webapi.BuiltIn;
using mbt.webapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace mbt.webapi.Services;

public class CurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool UseSystemAccount { get; set; } = false;

    public string UserId =>
        UseSystemAccount
            ? BuiltInConstants.SystemUserId
            : _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimConstants.ObjectId) ??
              BuiltInConstants.SystemUserId;

    public string UserName =>
        UseSystemAccount
            ? BuiltInConstants.SystemUserName
            : _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimConstants.Name) ??
              BuiltInConstants.SystemUserName;

    public string Email =>
        UseSystemAccount
            ? string.Empty
            : _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public bool IsInRole(string role)
    {
        if (UseSystemAccount) return true;
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext == null || httpContext.User.IsInRole(role);
    }

    public bool IsInRoles(IEnumerable<string> roles)
    {
        if (UseSystemAccount) return true;
        var httpContext = _httpContextAccessor.HttpContext;
        // temporary for daemon apps without HttpContext
        if (httpContext == null) return true;

        var hasRole = roles.Any(r => httpContext.User.IsInRole(r));

        return hasRole;
    }

    public bool IsInRoles(params string[] roles)
    {
        return IsInRoles(roles.AsEnumerable());
    }
}
